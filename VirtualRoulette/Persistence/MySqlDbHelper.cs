using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using VirtualRoulette.Domain;
using VirtualRoulette.Common;
using VirtualRoulette.Exceptions;
using static VirtualRoulette.Common.MySqlFunctions;

namespace VirtualRoulette.Persistence
{
    public sealed class MySqlDbHelper : IDbHelper
    {
        private readonly string _connectionString;

        public MySqlDbHelper(IConfiguration config)
        {
            _connectionString = config[Constants.ConnectionStringKey];
        }

        #region Users

        public Task<User> GetUserAsync(int id, User.ControlFlags flags) =>
            ConnectAsync(_connectionString, async connection =>
            {
                var command = new MySqlCommand($@"{ReusableQueries.GetUsersSql} WHERE id = @id LIMIT 1", connection);
                command.Parameters.AddWithValue("@id", id);
                var bets = flags.HasFlag(User.ControlFlags.Bets) ? await ListBetsAsync(id) : new List<UserBet>();

                return await command.QueryOneAsync(reader => reader.MapToUser(bets));
            });

        public Task<User> GetUserAsync(string username, User.ControlFlags flags) =>
            ConnectAsync(_connectionString, async connection =>
            {
                var command = new MySqlCommand($@"{ReusableQueries.GetUsersSql} WHERE username = @username LIMIT 1", connection);
                command.Parameters.AddWithValue("@username", username);

                var user = await command.QueryOneAsync(reader => reader.MapToUser());
                if (user == null)
                    return null;

                user.Bets = flags.HasFlag(User.ControlFlags.Bets) ? await ListBetsAsync(user.Id) : new List<UserBet>();

                return user;
            });

        public Task UpdateUserAsync(User user, int rowVersion) =>
            ConnectAsync(_connectionString, async connection =>
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    await UpdateUserAsync(user, rowVersion, connection, transaction);

                    if (!user.BetMade)
                        return;

                    await AddBet(user.Bets.OrderByDescending(bet => bet.DateCreated).First(), connection, transaction);

                    transaction.Commit();
                }
                catch (Exception) // Rollback transaction and rethrow exception with full stacktrace
                {
                    transaction.Rollback();
                    throw;
                }
            });

        private static async Task UpdateUserAsync(User user, int rowVersion, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (var command = new MySqlCommand(
                @"UPDATE user 
                            SET    username = @username,
                                   password = @password,
                                   firstName = @firstName, 
                                   lastName = @lastName, 
                                   balance = @balance, 
                                   dateModified = @dateModified,
                                   rowVersion = rowVersion + 1
                            WHERE  ( id = @id AND rowVersion = @rowVersion); ",
                connection,
                transaction))
            {
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@firstName", user.FirstName);
                command.Parameters.AddWithValue("@lastName", user.LastName);
                command.Parameters.AddWithValue("@balance", user.Balance);
                command.Parameters.AddWithValue("@dateModified", user.DateModified);

                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@rowVersion", rowVersion);

                var rows = await command.ExecuteNonQueryAsync();

                if (rows == 0) //Second optimistic concurrency check
                    throw new ConcurrencyException();
            }
        }

        #endregion

        #region Bets

        public Task<UserBet> GetUnProcessedBetAsync() =>
            ConnectAsync(_connectionString, connection =>
            {
                var command = new MySqlCommand($@"{ReusableQueries.GetBetsSql} WHERE processed=0 ORDER BY dateCreated ASC LIMIT 1", connection);
                return command.QueryOneAsync(reader => reader.MapToUserBet());
            });

        public Task UpdateBet(UserBet bet)
            => ConnectAsync(_connectionString, async conn =>
            {
                using (var command = new MySqlCommand(
                    @"UPDATE user_bet 
                         SET    userId = @userId, 
                                bet = @bet, 
                                amount = @amount, 
                                wonAmount = @wonAmount, 
                                winningNumber = @winningNumber, 
                                won = @won, 
                                dateCreated = @dateCreated, 
                                ipAddress = @ipAddress, 
                                processed = @processed
                        WHERE spinId = @spinId",
                    conn))
                {
                    command.Parameters.AddWithValue("@userId", bet.UserId);
                    command.Parameters.AddWithValue("@bet", bet.Bet);
                    command.Parameters.AddWithValue("@amount", bet.Amount);
                    command.Parameters.AddWithValue("@wonAmount", bet.WonAmount);
                    command.Parameters.AddWithValue("@winningNumber", bet.WinningNumber);
                    command.Parameters.AddWithValue("@won", bet.Won);
                    command.Parameters.AddWithValue("@ipAddress", bet.IpAddress);
                    command.Parameters.AddWithValue("@dateCreated", bet.DateCreated);
                    command.Parameters.AddWithValue("@processed", bet.Processed);

                    command.Parameters.AddWithValue("@spinId", bet.SpinId.ToString());

                    await command.ExecuteNonQueryAsync();
                }
            });

        private Task<List<UserBet>> ListBetsAsync(int userId)
            => ConnectAsync(_connectionString, connection =>
            {
                var command = new MySqlCommand($@"{ReusableQueries.GetBetsSql} WHERE userId=@userId", connection);
                command.Parameters.AddWithValue("@userId", userId);

                return command.QueryAsync(reader => reader.MapToUserBet());
            });

        private static async Task AddBet(UserBet bet, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (var command = new MySqlCommand(
                @"INSERT INTO user_bet 
                                (userId, 
                                 spinId, 
                                 bet, 
                                 amount, 
                                 wonAmount, 
                                 winningNumber, 
                                 won, 
                                 dateCreated, 
                                 ipAddress) 
                      VALUES    (@userId, 
                                 @spinId, 
                                 @bet, 
                                 @amount, 
                                 @wonAmount, 
                                 @winningNumber, 
                                 @won, 
                                 @dateCreated, 
                                 @ipAddress)",
                connection,
                transaction))
            {
                command.Parameters.AddWithValue("@userId", bet.UserId);
                command.Parameters.AddWithValue("@spinId", bet.SpinId.ToString());
                command.Parameters.AddWithValue("@bet", bet.Bet);
                command.Parameters.AddWithValue("@amount", bet.Amount);
                command.Parameters.AddWithValue("@wonAmount", bet.WonAmount);
                command.Parameters.AddWithValue("@winningNumber", bet.WinningNumber);
                command.Parameters.AddWithValue("@won", bet.Won);
                command.Parameters.AddWithValue("@ipAddress", bet.IpAddress);
                command.Parameters.AddWithValue("@dateCreated", bet.DateCreated);

                await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region AddJackpot

        public Task<Jackpot> GetJackpotAsync() =>
            ConnectAsync(_connectionString, connection =>
            {
                var command = new MySqlCommand($@"{ReusableQueries.GetJackpotSql} ORDER BY dateCreated DESC LIMIT 1", connection);
                return command.QueryOneAsync(reader => reader.MapToJackpot());
            });

        public Task<List<Jackpot>> ListJackpotsAsync() =>
            ConnectAsync(_connectionString, connection =>
            {
                var command = new MySqlCommand($@"{ReusableQueries.GetJackpotSql} ORDER BY dateCreated ASC", connection);
                return command.QueryAsync(reader => reader.MapToJackpot());
            });

        public Task AddJackPotAsync(Jackpot jackpot)
            => ConnectAsync(_connectionString, async connection =>
                {
                    var command = new MySqlCommand(@"INSERT INTO jackpot_history (spinId,amount,dateCreated) VALUES (@spinId,@amount,@dateCreated);", connection);

                    command.Parameters.AddWithValue("@spinId", jackpot.SpinId.ToString());
                    command.Parameters.AddWithValue("@amount", jackpot.Amount);
                    command.Parameters.AddWithValue("@dateCreated", jackpot.DateCreated);

                    await command.ExecuteNonQueryAsync();
                });

        #endregion
    }
}
