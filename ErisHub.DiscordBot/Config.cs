using System.Collections.Generic;

namespace ErisHub.DiscordBot
{
    public class Config
    {
        public string Token { get; set; }
        public List<Server> Servers { get; set; }

        public string HubApiUrl { get; set; }

        public ulong StatusChannelId { get; set; }

        public BotSettings BotSettings { get; set; }
    }

    public class Server
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }
    }

    public class BotSettings
    {
        public ulong MentionRoleId { get; set; }
        public ulong BotStatusChannelId { get; set; }
    }
}
