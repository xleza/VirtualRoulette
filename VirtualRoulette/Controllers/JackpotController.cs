using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualRoulette.Common;
using VirtualRoulette.Models;
using VirtualRoulette.Persistence;

namespace VirtualRoulette.Controllers
{
    [Route("api/jackpot")]
    [ApiController]
    [Authorize]
    public sealed class JackpotController : ControllerBase
    {
        private readonly IDbHelper _dbHelper;

        public JackpotController(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        [HttpGet("current")]
        public async Task<ActionResult<decimal>> GetCurrent()
            => Ok(await _dbHelper.GetJackpotAsync().Map(jackpot => jackpot?.Amount ?? 0));

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<JackpotHistoryDto>>> GetGameHistory()
            => Ok(await _dbHelper.ListJackpotsAsync()
                .Map(jackpots => jackpots.Select(JackpotHistoryDto.Create)));
    }
}
