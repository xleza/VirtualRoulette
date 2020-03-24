using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualRoulette.Commands;
using VirtualRoulette.Security;

namespace VirtualRoulette.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize]
    public sealed class AuthController : ControllerBase
    {

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]AuthenticateUser cmd, [FromServices] ISecurityService service)
        {
            await service.AuthenticateAsync(cmd);

            return NoContent();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return NoContent();
        }
    }
}
