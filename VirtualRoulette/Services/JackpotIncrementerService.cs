using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VirtualRoulette.Common;
using VirtualRoulette.Domain;
using VirtualRoulette.Persistence;

namespace VirtualRoulette.Services
{
    public sealed class JackpotIncrementerService : IScopedProcessingService
    {
        private readonly IDbHelper _dbHelper;
        private readonly ILogger<JackpotIncrementerService> _logger;
        private readonly int _jackpotCheckDelayInMilliseconds;

        public JackpotIncrementerService(IDbHelper dbHelper, ILogger<JackpotIncrementerService> logger, IConfiguration config)
        {
            _dbHelper = dbHelper;
            _logger = logger;

            if (!int.TryParse(config[Constants.JackpotCheckDelayInMillisecondsKey], out _jackpotCheckDelayInMilliseconds))
                throw new ArgumentException($"Can't parse value of {nameof(Constants.JackpotCheckDelayInMillisecondsKey)}");
        }

        private async Task IncrementJackpotAsync()
        {
            var bet = await _dbHelper.GetUnProcessedBetAsync(); //Fetch the bet of which amount didn't affect a jackpot
            if (bet == null)
                return;

            var jackPot = await _dbHelper.GetJackpotAsync();

            var jackPotNewAmount = (jackPot?.Amount ?? 0) + bet.Amount / 100; //Calculate jackpot amount

            await _dbHelper.AddJackPotAsync(new Jackpot  //I am using append-only approach to have track of changes and full history of jackpot entity
            {
                SpinId = bet.SpinId,
                Amount = jackPotNewAmount,
                DateCreated = DateTime.Now
            });

            bet.Processed = true;
            await _dbHelper.UpdateBet(bet);

            _logger.LogInformation($"Jackpot new amount is {jackPotNewAmount}");

            //Push notifications to all connected clients
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var transactionScope = new TransactionScope()) //This way code in the block participates in a transaction and we don't need to write extra transactions in inner scope
                    {
                        await IncrementJackpotAsync();
                        transactionScope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured during IncrementJackPot");
                }

                await Task.Delay(_jackpotCheckDelayInMilliseconds, stoppingToken);
            }
        }
    }
}
