using System;

namespace VirtualRoulette.Domain
{
    public sealed class UserBet
    {
        public int UserId { get; set; }
        public Guid SpinId { get; set; }
        public string Bet { get; set; }
        public decimal Amount { get; set; }
        public decimal? WonAmount { get; set; }
        public int WinningNumber { get; set; }
        public bool Won { get; set; }
        public string IpAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Processed { get; set; }
    }
}
