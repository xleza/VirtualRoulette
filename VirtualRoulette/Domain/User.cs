using System;
using ge.singular.roulette;
using VirtualRoulette.Commands;
using VirtualRoulette.Exceptions;

namespace VirtualRoulette.Domain
{
    public sealed partial class User
    {
        public delegate int EstimateWin(string bet, int winingNumber); // using delegate to decouple domain logic to external lib and better testing

        public void MakeBat(MakeBet cmd, EstimateWin estimateWin, int winningNumber, long betAmount, string ipAddress)
        {
            if (betAmount > Balance)
                throw new BadRequestException("Not enough in balance");

            var estWin = estimateWin(cmd.Bet, winningNumber);
            var won = estWin > 0;

            if (betAmount > Balance)
                throw new BadRequestException("Not enough in balance");

            Balance = won ? Balance - betAmount + estWin : Balance - betAmount;

            Bets.Add(new UserBet
            {
                UserId = Id,
                Bet = cmd.Bet,
                Amount = betAmount,
                WonAmount = won ? estWin : default(int?),
                SpinId = Guid.NewGuid(),
                Won = won,
                WinningNumber = winningNumber,
                IpAddress = ipAddress,
                DateCreated = DateTime.Now
            });

            BetMade = true;
        }
    }
}
