using System.ComponentModel.DataAnnotations;

namespace VirtualRoulette.Commands
{
    public sealed class MakeBet
    {
        [Required]
        public string Bet { get; set; }
        public int RowVersion { get; set; }
    }
}
