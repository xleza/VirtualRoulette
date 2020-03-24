namespace VirtualRoulette.Persistence
{
    public class ReusableQueries
    {
        public const string GetUsersSql = @"SELECT 
                                                  id, 
                                                  username, 
                                                  password, 
                                                  firstName, 
                                                  lastName,
                                                  balance,
                                                  dateModified,
                                                  rowVersion FROM user";

        public const string GetBetsSql = @"SELECT 
                                                 spinId,
                                                 userId,
                                                 bet,
                                                 amount,
                                                 wonAmount,
                                                 winningNumber,
                                                 won,
                                                 dateCreated,
                                                 ipAddress,
                                                 processed FROM user_bet";

        public const string GetJackpotSql = @"SELECT 
                                               spinId, 
                                               amount, 
                                               dateCreated 
                                               from jackpot_history";
    }
}
