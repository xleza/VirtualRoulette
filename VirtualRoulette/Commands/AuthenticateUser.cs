using System.ComponentModel.DataAnnotations;

namespace VirtualRoulette.Commands
{
    public class AuthenticateUser
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
