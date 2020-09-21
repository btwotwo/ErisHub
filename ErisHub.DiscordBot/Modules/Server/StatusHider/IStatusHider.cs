using ErisHub.DiscordBot.ApiClient;

namespace ErisHub.DiscordBot.Modules.Server
{
    public interface IStatusHider
    {
        bool Hide(StatusModel status);
        bool Show(StatusModel status);
        bool Hidden(StatusModel status);
    }
}