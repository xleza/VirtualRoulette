using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualRoulette.Commands;
using VirtualRoulette.Common;
using VirtualRoulette.Models;
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
        private readonly IDbHelper _dbHelper;
        private readonly ISecurityService _securityService;
        private readonly UsersCommandService _service;

        public UsersController(IDbHelper dbHelper, ISecurityService securityService, UsersCommandService service)
        {
            _dbHelper = dbHelper;
            _securityService = securityService;
            _service = service;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
            => Ok(await _dbHelper.GetUserAsync(_securityService.CurrentUser.Id, Domain.User.ControlFlags.Basic)
                .Map(UserProfileDto.Create));

        [HttpGet("balance")]
        public async Task<ActionResult<decimal>> GetBalance()
            => Ok(await _dbHelper.GetUserAsync(_securityService.CurrentUser.Id, Domain.User.ControlFlags.Basic)
                .Map(user => user.Balance));

        [HttpGet("game-history")]
        public async Task<ActionResult<IEnumerable<UserGameHistoryDto>>> GetGameHistory()
            => Ok(await _dbHelper.GetUserAsync(_securityService.CurrentUser.Id, Domain.User.ControlFlags.Bets)
                .Map(user => user.Bets.Select(UserGameHistoryDto.Create).OrderBy(story => story.DateCreated)));

        [HttpPost("make-bet")]
        public async Task<ActionResult<MakeBetResponseDto>> MakeBet([FromBody] MakeBet cmd)
            => Ok(await _service.MakeBet(cmd, Request.HttpContext.Connection.RemoteIpAddress.ToString()));
    }
}
