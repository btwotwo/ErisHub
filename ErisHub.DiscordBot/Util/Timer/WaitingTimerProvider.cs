using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ErisHub.DiscordBot.Util.Timer
{
    public interface IWaitingTimerProvider
    {
        public IWaitingTimer GetOrCreate(Guid id);
    }

    public class WaitingTimerProvider: IWaitingTimerProvider
    {
        private ConcurrentDictionary<Guid, IWaitingTimer> _timers;

        public WaitingTimerProvider()
        {
            _timers = new ConcurrentDictionary<Guid, IWaitingTimer>();
        }

        public IWaitingTimer GetOrCreate(Guid id)
        {
            return _timers.GetOrAdd(id, _ => new WaitingTimer());
        }
    }
}
