using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualRoulette.Domain;

namespace VirtualRoulette.Persistence
{
    public interface IDbHelper
    {
        Task<User> GetUserAsync(int id, User.ControlFlags flags);
        Task<User> GetUserAsync(string username, User.ControlFlags flags);
        Task UpdateUserAsync(User user, int rowVersion);
        Task<UserBet> GetUnProcessedBetAsync();
        Task<Jackpot> GetJackpotAsync();
        Task<List<Jackpot>> ListJackpotsAsync();
        Task AddJackPotAsync(Jackpot jackPot);
        Task UpdateBet(UserBet bet);
    }
}
