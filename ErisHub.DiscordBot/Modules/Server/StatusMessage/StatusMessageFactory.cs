using Discord;

namespace ErisHub.DiscordBot.Modules.Server
{
    public class StatusMessageFactory : IStatusMessageFactory
    {
        private readonly IDiscordClient _discordClient;

        public StatusMessageFactory(IDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public IStatusMessage Create(ulong channelId)
        {
            return new StatusMessage(channelId, _discordClient);
        }
    }
}
