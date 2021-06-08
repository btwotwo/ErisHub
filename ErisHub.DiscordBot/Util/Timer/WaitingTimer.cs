using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Util.Timer
{
    public interface IWaitingTimer
    {
        public bool RunPeriodically(Func<Task> callback, TimeSpan timeSpan, CancellationToken cancellationToken = default);
        public bool IsRunning();
    }

    public class WaitingTimer : IWaitingTimer
    {
        private bool _running = false;

        public bool IsRunning()
        {
            return _running;
        }

        public bool RunPeriodically(Func<Task> callback, TimeSpan timeSpan, CancellationToken cancellationToken = default)
        {
            if (IsRunning())
            {
                return false;
            }

            _running = true;
            cancellationToken.Register(() => _running = false);

            Task.Run(async () => await ProcessTask(callback, timeSpan, cancellationToken), cancellationToken);

            return true;
        }

        private async Task ProcessTask(Func<Task> callback, TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    await callback();
                    await Task.Delay(timeSpan);
                }
            }
            catch (OperationCanceledException)
            {
                _running = false;
            }
        }
    }
}
