using System;
using System.Threading.Tasks;
using ge.singular.roulette;
using VirtualRoulette.Commands;
using VirtualRoulette.Domain;
using VirtualRoulette.Exceptions;
using VirtualRoulette.Models;
using VirtualRoulette.Persistence;
using VirtualRoulette.Security;

namespace VirtualRoulette.Services
{
    public sealed class UsersCommandService
    {
        private readonly IDbHelper _repository;
        private readonly ISecurityService _securityService;

        public UsersCommandService(IDbHelper repository, ISecurityService securityService)
        {
            _repository = repository;
            _securityService = securityService;
        }

        public async Task<MakeBetResponseDto> MakeBet(MakeBet cmd, string ipAddress)
        {
            var validationResult = CheckBets.IsValid(cmd.Bet);
            if (!validationResult.getIsValid())
                throw new BadRequestException(Constants.InvalidBetExceptionTest);

            var user = await _repository.GetUserAsync(_securityService.CurrentUser.Id, User.ControlFlags.Basic | User.ControlFlags.Bets);
            if (user.RowVersion != cmd.RowVersion)
                throw new ConcurrencyException();

            var winningNumber = new Random().Next(0, 36);
            var (spinId, won, wonAmount) = user.MakeBat(cmd, CheckBets.EstimateWin, winningNumber, validationResult.getBetAmount(), ipAddress);

            await _repository.UpdateUserAsync(user, cmd.RowVersion);

            return new MakeBetResponseDto
            {
                WinningNumber = winningNumber,
                Won = won,
                SpinId = spinId,
                WonAmount = wonAmount,
            };
        }
    }
}
