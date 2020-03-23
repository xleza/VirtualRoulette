using System;
using System.Threading.Tasks;
using ge.singular.roulette;
using VirtualRoulette.Commands;
using VirtualRoulette.Domain;
using VirtualRoulette.Exceptions;
using VirtualRoulette.Persistence;
using VirtualRoulette.Security;

namespace VirtualRoulette.Services
{
    public sealed class UsersCommandService
    {
        private readonly IUsersRepository _repository;
        private readonly ISecurityService _securityService;

        public UsersCommandService(IUsersRepository repository, ISecurityService securityService)
        {
            _repository = repository;
            _securityService = securityService;
        }

        public async Task MakeBet(MakeBet cmd, string ipAddress)
        {
            var validationResult = CheckBets.IsValid(cmd.Bet);
            if (!validationResult.getIsValid())
                throw new BadRequestException("Invalid bet");

            var user = await _repository.GetAsync(_securityService.CurrentUser.Id, User.ControlFlags.Basic | User.ControlFlags.Bets);
            if (user.RowVersion != cmd.RowVersion)
                throw new ConcurrencyException();

            user.MakeBat(cmd, CheckBets.EstimateWin, new Random().Next(0, 36), validationResult.getBetAmount(), ipAddress);

            await _repository.UpdateAsync(user, cmd.RowVersion);
        }
    }
}
