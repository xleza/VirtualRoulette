using System;
using System.Collections.Generic;
using VirtualRoulette.Commands;
using VirtualRoulette.Exceptions;

namespace VirtualRoulette.Domain
{
    public sealed partial class User
    {
        public delegate int EstimateWin(string bet, int winingNumber); // Using delegate to decouple domain logic to external lib and to test better

        public (Guid spinId, bool won, decimal? wonAmount) MakeBat(MakeBet cmd, EstimateWin estimateWin, int winningNumber, long betAmount, string ipAddress)
        {
            if (betAmount > Balance)
                throw new BadRequestException(Constants.NotEnoughBalanceExceptionText);

            var estWin = estimateWin(cmd.Bet, winningNumber);
            var won = estWin > 0;
            var wonAmount = won ? estWin : default(int?);
            var spinId = Guid.NewGuid();

            Balance = won ? Balance - betAmount + estWin : Balance - betAmount;

            if (Bets == null)
                Bets = new List<UserBet>();

            Bets.Add(new UserBet
            {
                UserId = Id,
                Bet = cmd.Bet,
                Amount = betAmount,
                WonAmount = wonAmount,
                SpinId = spinId,
                Won = won,
                WinningNumber = winningNumber,
                IpAddress = ipAddress,
                DateCreated = DateTime.Now
            });

            BetMade = true;

            return (spinId, won, wonAmount);
        }
    }
}
