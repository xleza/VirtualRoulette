using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using VirtualRoulette.Domain;
using VirtualRoulette.Common;
using VirtualRoulette.Exceptions;
using static VirtualRoulette.Common.ConnectionHelper;

namespace VirtualRoulette.Persistence
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string GetUsersQuerySql = @"SELECT 
                                                  id, 
                                                  username, 
                                                  password, 
                                                  firstName, 
                                                  lastName,
                                                  balance,
                                                  dateModified,
                                                  rowVersion FROM user";

        private const string GetBestQuerySql = @"SELECT 
                                                 spinId,
                                                 userId,
                                                 bet,
                                                 amount,
                                                 wonAmount,
                                                 winningNumber,
                                                 won,
                                                 dateCreated,
                                                 ipAddress FROM user_bet";

        public Task<User> GetAsync(int id, User.ControlFlags flags) =>
            ConnectAsync(_connectionString,
                async con =>
                {
                    var command = new MySqlCommand($@"{GetUsersQuerySql} WHERE id = @id LIMIT 1", con);
                    command.Parameters.AddWithValue("@id", id);
                    var reader = await command.ExecuteReaderAsync();

                    if (!reader.HasRows) return null;

                    await reader.ReadAsync();

                    return new User
                    {
                        Id = reader.GetInt32("id"),
                        Username = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Balance = reader.GetInt64("balance"),
                        DateModified = reader.GetDateTimeOrNull("dateModified"),
                        RowVersion = reader.GetInt32("rowVersion"),
                        Bets = flags.HasFlag(User.ControlFlags.Bets)
                            ? await ListBetsAsync(reader.GetInt32("id"))
                            : new List<UserBet>()
                    };
                });

        public Task<User> GetAsync(string username, User.ControlFlags flags) =>
            ConnectAsync(_connectionString,
                async con =>
                {
                    var command = new MySqlCommand($@"{GetUsersQuerySql} WHERE username = @username LIMIT 1", con);
                    command.Parameters.AddWithValue("@username", username);
                    var reader = await command.ExecuteReaderAsync();

                    if (!reader.HasRows) return null;

                    await reader.ReadAsync();

                    return new User
                    {
                        Id = reader.GetInt32("id"),
                        Username = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Balance = reader.GetInt64("balance"),
                        DateModified = reader.GetDateTimeOrNull("dateModified"),
                        RowVersion = reader.GetInt32("rowVersion"),
                        Bets = flags.HasFlag(User.ControlFlags.Bets)
                            ? await ListBetsAsync(reader.GetInt32("id"))
                            : new List<UserBet>()
                    };
                });

        private Task<List<UserBet>> ListBetsAsync(int userId)
            => ConnectAsync(_connectionString,
                async con =>
                {
                    var command = new MySqlCommand($@"{GetBestQuerySql} WHERE userId=@userId", con);
                    command.Parameters.AddWithValue("@userId", userId);
                    var reader = await command.ExecuteReaderAsync();

                    var result = new List<UserBet>();

                    while (await reader.ReadAsync())
                    {
                        result.Add(new UserBet
                        {
                            SpinId = Guid.Parse(reader.GetString("spinId")),
                            UserId = reader.GetInt32("userId"),
                            Bet = reader.GetString("bet"),
                            Amount = reader.GetInt64("amount"),
                            WonAmount = reader.GetInt64OrNull("wonAmount"),
                            WinningNumber = reader.GetInt32("winningNumber"),
                            Won = reader.GetBoolean("won"),
                            IpAddress = reader.GetString("ipAddress"),
                            DateCreated = reader.GetDateTime("DateCreated")
                        });
                    }

                    return result;
                });

        public Task UpdateAsync(User user, int rowVersion) =>
            ConnectAsync(_connectionString, async conn =>
            {
                var transaction = conn.BeginTransaction();
                try
                {
                    await UpdateAsync(user, rowVersion, conn, transaction);

                    if (!user.BetMade)
                        return;

                    await AddBet(user.Bets.OrderByDescending(bet => bet.DateCreated).First(), conn, transaction);

                    transaction.Commit();
                }
                catch (Exception) // Rollback transaction and re throw exception with full stacktrace
                {
                    transaction.Rollback();
                    throw;
                }
            });

        private static async Task UpdateAsync(User user, int rowVersion, MySqlConnection connection, MySqlTransaction transaction)
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

        private static async Task AddBet(UserBet bet, MySqlConnection connection, MySqlTransaction transaction) // We need only one iteration over bets and IEnumerable is enough to it. With this abstraction we can pass List,Array,Hashset and etc.
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
    }
}
