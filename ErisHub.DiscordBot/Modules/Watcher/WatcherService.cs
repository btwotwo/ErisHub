using System;
using System.Collections.Generic;
using System.Linq;
using ErisHub.DiscordBot.Database;
using Microsoft.EntityFrameworkCore.Internal;

namespace ErisHub.DiscordBot.Modules.Watcher
{
    public class WatcherService
    {
        private readonly BotContext _db;
        private readonly HashSet<string> _watchingSet;


        public WatcherService(BotContext db)
        {
            _db = db;
            _watchingSet = new HashSet<string>();
        }

        public void AddToWatch(string serverId)
        {
            if (!_db.WatcherSettings.Any(x => x.Server == serverId))
            {
                throw new WatcherException("Server does not exist.");
            }

            _watchingSet.Add(serverId);
        }

        public void RemoveFromWatch(string serverId)
        {
            if (_watchingSet.Contains(serverId))
            {
                _watchingSet.Remove(serverId);
            }
        }
    }

    public class WatcherException : Exception
    {
        public WatcherException(string message) : base(message)
        {
        }
    }
}
