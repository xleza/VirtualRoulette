using System.Threading.Tasks;
using VirtualRoulette.Commands;

namespace VirtualRoulette.Security
{
    public interface ISecurityService // Create interface for mocking
    {
        SecurityUser CurrentUser { get; }

        Task Authenticate(AuthenticateUser cmd);
    }
}
