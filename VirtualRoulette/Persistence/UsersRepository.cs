using System.Threading.Tasks;
using VirtualRoulette.Domain;

namespace VirtualRoulette.Persistence
{
    public sealed class UsersRepository : IUsersRepository
    {
        public Task<User> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Get(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}
