using System;
using VirtualRoulette.Domain;

namespace VirtualRoulette.Models
{
    public class JackpotHistoryDto
    {
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }

        public static JackpotHistoryDto Create(Jackpot jackpot)
            => new JackpotHistoryDto
            {
                Amount = jackpot.Amount,
                DateCreated = jackpot.DateCreated
            };
    }
}
