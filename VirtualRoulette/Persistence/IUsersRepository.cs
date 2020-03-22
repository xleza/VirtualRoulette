using System.Threading.Tasks;
using VirtualRoulette.Domain;

namespace VirtualRoulette.Persistence
{
    public interface IUsersRepository
    {
        Task<User> Get(int id);
        Task<User> Get(string username);
    }
}
