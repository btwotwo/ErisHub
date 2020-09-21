using System;

namespace ErisHub.DiscordBot.Modules.Server
{
    public class StatusException : Exception
    {
        public StatusException(string msg) : base(msg) { }
    }

    public class InvalidChannelStatusException : StatusException
    {
        public InvalidChannelStatusException(ulong channelId) : base($"Could't find channel with '{channelId}'. Please check configuration.") { }
    }
}
