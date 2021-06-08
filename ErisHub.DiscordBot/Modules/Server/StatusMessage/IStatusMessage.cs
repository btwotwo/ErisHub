using ErisHub.DiscordBot.ApiClient;
using ErisHub.DiscordBot.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Modules.Server
{
    public interface IStatusMessage
    {
        Task<ulong> TryCreateFromIdAsync(ulong id);
        Task UpdateAsync(IEnumerable<StatusModel> statuses);
    }
}
