using System;

namespace VirtualRoulette.Models
{
    public sealed class MakeBetResponseDto
    {
        public Guid SpinId { get; set; }
        public bool Won { get; set; }
        public int WinningNumber { get; set; }
        public decimal? WonAmount { get; set; }
    }
}
