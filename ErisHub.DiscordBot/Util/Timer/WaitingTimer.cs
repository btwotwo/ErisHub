using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Util.Timer
{
    public interface IWaitingTimer
    {
        public bool RunPeriodically(Func<Task> callback, TimeSpan timeSpan, CancellationToken cancellationToken);

        public bool IsRunning();
    }

    public class WaitingTimer : IWaitingTimer
    {
        private bool _running = false;

        public bool IsRunning()
        {
            return _running;
        }

        public bool RunPeriodically(Func<Task> callback, TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            if (IsRunning())
            {
                return false;
            }

            _running = true;
            cancellationToken.Register(() => _running = false);

            var task = Task.Factory.StartNew(async (_) =>
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
            }, cancellationToken, TaskCreationOptions.LongRunning);

            return true;
        }
    }
}
