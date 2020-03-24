using System;
using VirtualRoulette.Domain;

namespace VirtualRoulette.Models
{
    public class UserGameHistoryDto
    {
        public Guid SpinId { get; set; }
        public decimal BetAmount { get; set; }
        public decimal? WonAmount { get; set; }
        public bool Won { get; set; }
        public DateTime DateCreated { get; set; }

        public static UserGameHistoryDto Create(UserBet bet)
            => new UserGameHistoryDto
            {
                SpinId = bet.SpinId,
                BetAmount = bet.Amount,
                WonAmount = bet.WonAmount,
                Won = bet.Won,
                DateCreated = bet.DateCreated
            };
    }
}
