using System;

namespace VirtualRoulette.Domain
{
    public sealed class UserBet
    {
        public int UserId { get; set; }
        public Guid SpinId { get; set; }
        public string Bet { get; set; }
        public long Amount { get; set; }
        public long? WonAmount { get; set; }
        public int WinningNumber { get; set; }
        public bool Won { get; set; }
        public string IpAddress { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
