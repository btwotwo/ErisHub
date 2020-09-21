using Discord;
using Discord.Rest;
using Discord.WebSocket;
using ErisHub.DiscordBot.Util;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Modules.Server
{
    public interface IStatusUpdater
    {
        Task<Error?> StartAsync();
        Task<Error?> StopAsync();
    }
}