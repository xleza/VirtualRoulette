using System;

namespace VirtualRoulette.Domain
{
    public sealed class Jackpot
    {
        public Guid SpinId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
