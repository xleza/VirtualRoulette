using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualRoulette.Commands;
using VirtualRoulette.Common;
using VirtualRoulette.Persistence;
using VirtualRoulette.Security;
using VirtualRoulette.Services;

namespace VirtualRoulette.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public sealed class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ISecurityService _securityService;
        private readonly UsersCommandService _service;

        public UsersController(IUsersRepository usersRepository, ISecurityService securityService, UsersCommandService service)
        {
            _usersRepository = usersRepository;
            _securityService = securityService;
            _service = service;
        }

        [HttpGet("balance")]
        public Task<long> GetBalance() => _usersRepository.GetAsync(_securityService.CurrentUser.Id, Domain.User.ControlFlags.Basic).Map(user => user.Balance);

        [HttpPost("make-bet")]
        public async Task<ActionResult> MakeBet([FromBody] MakeBet cmd)
        {
            await _service.MakeBet(cmd, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            return Ok();
        }
    }
}
