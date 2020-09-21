namespace ErisHub.DiscordBot.Modules.Server
{
    public interface IStatusMessageFactory
    {
        IStatusMessage Create(ulong channelId);
    }
}
