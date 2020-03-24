using System;
using System.Collections.Generic;
using System.Data.Common;
using VirtualRoulette.Common;
using VirtualRoulette.Domain;

namespace VirtualRoulette.Persistence
{
    public static class Mapper
    {
        public static User MapToUser(this DbDataReader self, List<UserBet> bets = null)
        {
            if (self == null)
                return null;

            return new User
            {
                Id = self.GetInt32("id"),
                Username = self.GetString("username"),
                Password = self.GetString("password"),
                FirstName = self.GetString("firstName"),
                LastName = self.GetString("lastName"),
                Balance = self.GetDecimal("balance"),
                DateModified = self.GetDateTimeOrNull("dateModified"),
                RowVersion = self.GetInt32("rowVersion"),
                Bets = bets ?? new List<UserBet>()
            };
        }

        public static UserBet MapToUserBet(this DbDataReader self)
        {
            if (self == null)
                return null;

            return new UserBet
            {
                SpinId = Guid.Parse(self.GetString("spinId")),
                UserId = self.GetInt32("userId"),
                Bet = self.GetString("bet"),
                Amount = self.GetDecimal("amount"),
                WonAmount = self.GetDecimalOrNull("wonAmount"),
                WinningNumber = self.GetInt32("winningNumber"),
                Won = self.GetBoolean("won"),
                IpAddress = self.GetString("ipAddress"),
                DateCreated = self.GetDateTime("dateCreated"),
                Processed = self.GetBoolean("processed")
            };
        }

        public static Jackpot MapToJackpot(this DbDataReader self)
        {
            if (self == null)
                return null;

            return new Jackpot
            {
                SpinId = Guid.Parse(self.GetString("spinId")),
                Amount = self.GetDecimal("amount"),
                DateCreated = self.GetDateTime("dateCreated")
            };
        }
    }
}
