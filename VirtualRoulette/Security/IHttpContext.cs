using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VirtualRoulette.Security
{
    public interface IHttpContext
    {
        bool UserIsAuthenticated { get; }
        IEnumerable<Claim> UserClaims { get; }

        Task SignInAsync(string scheme, ClaimsPrincipal claimsPrincipal);
    }
}
