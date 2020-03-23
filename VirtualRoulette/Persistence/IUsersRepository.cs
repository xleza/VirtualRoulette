using System.Threading.Tasks;
using VirtualRoulette.Domain;

namespace VirtualRoulette.Persistence
{
    public interface IUsersRepository
    {
        Task<User> GetAsync(int id, User.ControlFlags flags);
        Task<User> GetAsync(string username, User.ControlFlags flags);
        Task UpdateAsync(User user, int rowVersion);
    }
}
