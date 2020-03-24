using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace VirtualRoulette.Security
{
    public class HttpContextAdapter : IHttpContext
    {
        private readonly HttpContext _httpContext;

        public HttpContextAdapter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public bool UserIsAuthenticated => _httpContext.User?.Identity.IsAuthenticated == true;
        public IEnumerable<Claim> UserClaims => _httpContext.User.Claims;
        public Task SignInAsync(string scheme, ClaimsPrincipal claimsPrincipal) => _httpContext.SignInAsync(scheme, claimsPrincipal);
    }
}
