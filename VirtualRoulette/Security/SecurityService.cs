using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using VirtualRoulette.Commands;
using VirtualRoulette.Common;
using VirtualRoulette.Domain;
using VirtualRoulette.Exceptions;
using VirtualRoulette.Persistence;

namespace VirtualRoulette.Security
{
    public sealed class SecurityService : ISecurityService
    {
        private readonly HttpContext _httpContext;
        private readonly IDbHelper _usersRepository;

        private static class ClaimKeys
        {
            public const string Id = nameof(Id);
            public const string Username = nameof(Username);
        }

        public SecurityService(IHttpContextAccessor httpContext, IDbHelper usersRepository)
        {
            _httpContext = httpContext.HttpContext;
            _usersRepository = usersRepository;
        }

        public SecurityUser CurrentUser
        {
            get
            {
                var claims = GetClaims();

                claims.TryGetValue(ClaimKeys.Id, out var userid);
                claims.TryGetValue(ClaimKeys.Username, out var username);

                if (userid?.Value == null || username?.Value == null)
                    return null;

                return new SecurityUser(int.Parse(userid.Value), username.Value); //I am sure that userId always be int and therefore I am int.Parse instead of TryParse
            }
        }

        public async Task Authenticate(AuthenticateUser cmd)
        {
            var user = await _usersRepository.GetUserAsync(cmd.Username, User.ControlFlags.Basic);

            if (user == null || !user.Password.Equals(Cryptography.EncryptPassword(cmd.Password)))
                throw new BadRequestException(Constants.InvalidUsernameOrPasswordExceptionText);

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimKeys.Id, user.Id.ToString()),
                new Claim(ClaimKeys.Username, user.Username)
            }, Constants.CookiesKey);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await _httpContext.SignInAsync(Constants.CookiesKey, claimsPrincipal);
        }


        private Dictionary<string, Claim> GetClaims() //Used dictionary to access claim value by key in O(1) time complexity
        {
            if (_httpContext.User?.Identity.IsAuthenticated != true)
                return new Dictionary<string, Claim>();

            return _httpContext.User.Claims.ToDictionary(claim => claim.Type, claim => claim);
        }
    }
}
