using ErisHub.DiscordBot.ApiClient;
using ErisHub.DiscordBot.Util;
using System.Collections.Generic;

namespace ErisHub.DiscordBot.Modules.Server
{
    public class StatusHider : IStatusHider
    {
        private readonly HashSet<string> _hidden;
        public StatusHider() => _hidden = new HashSet<string>(); //todo replace with db

        public bool Hidden(StatusModel status) => _hidden.Contains(status.Name);

        public bool Hide(StatusModel status) => _hidden.Add(status.Name);

        public bool Show(StatusModel status) => _hidden.Remove(status.Name);
    }
}