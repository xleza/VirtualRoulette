using VirtualRoulette.Domain;

namespace VirtualRoulette.Models
{
    public sealed class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public int RowVersion { get; set; }

        public static UserProfileDto Create(User user)
            => new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Balance = user.Balance,
                RowVersion = user.RowVersion
            };
    }
}
