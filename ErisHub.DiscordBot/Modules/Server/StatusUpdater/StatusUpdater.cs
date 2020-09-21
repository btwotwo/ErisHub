using ErisHub.DiscordBot.ApiClient;
using ErisHub.DiscordBot.Util;
using ErisHub.DiscordBot.Util.Timer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Modules.Server
{
    public class StatusUpdater : IStatusUpdater
    {
        private static readonly Guid StatusServiceId = new Guid("F1CAB9AF-A8F6-4CE8-AB20-CA40A5AEF1C7");

        private readonly ILogger _logger;
        private readonly IWaitingTimer _waitingTimer;
        private readonly IStatusMessage _statusMessage;
        private readonly IStatusHider _statusHider;
        private readonly IStatusProvider _statusProvider;
        private CancellationTokenSource _tokenSource;

        public StatusUpdater(ILoggerFactory loggerFactory, IWaitingTimerProvider waitingTimerProvider, Config config, IStatusMessageFactory statusMessageFactory, IStatusHider statusHider, IStatusProvider statusProvider)
        {
            _logger = loggerFactory.CreateLogger<StatusUpdater>();
            _waitingTimer = waitingTimerProvider.GetOrCreate(StatusServiceId);
            _statusMessage = statusMessageFactory.Create(config.StatusChannelId);
            _statusHider = statusHider;
            _statusProvider = statusProvider;
        }

        public async Task<Error?> StartAsync()
        {
            if (_waitingTimer.IsRunning())
            {
                return new Error("Already started.");
            }

            await _statusMessage.TryCreateFromIdAsync(0); //todo save message id to database

            _tokenSource = new CancellationTokenSource();
            _waitingTimer.RunPeriodically(UpdateAsync, TimeSpan.FromSeconds(10), _tokenSource.Token);

            return null;
        }
        
        public async Task<Error?> StopAsync()
        {
            if (_waitingTimer.IsRunning() == false)
            {
                return new Error("Not running.");
            }

            _tokenSource.Cancel();
            _tokenSource.Dispose();

            return null;
        }

        private async Task UpdateAsync()
        {
            try
            {
                var statuses = await UpdateStatuses();
                await _statusMessage.UpdateAsync(statuses);
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Error while updating message");
            }
        }

        private async Task<IEnumerable<StatusModel>> UpdateStatuses()
        {
            try
            {
                var statuses = (await _statusProvider.GetStatusesAsync()).ToArray();
                return statuses.Where(x => _statusHider.Hidden(x) == false).OrderBy(x => x.Name);
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Failed to retrieve server list.");
                return Enumerable.Empty<StatusModel>();
            }
        }
    }
}