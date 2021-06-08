using ErisHub.DiscordBot.ApiClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Modules.Server
{
    public interface IStatusProvider
    {
        Task<StatusModel?> GetByNameOrDefaultAsync(string name);
        Task<IEnumerable<StatusModel>> GetStatusesAsync();
    }
    public class StatusProvider : IStatusProvider
    {
        private readonly ServersClient _serversClient;

        public StatusProvider(ServersClient serversClient)
        {
            _serversClient = serversClient;
        }

        public async Task<StatusModel?> GetByNameOrDefaultAsync(string name) => await _serversClient.GetSingleServerStatusAsync(name);
        public async Task<IEnumerable<StatusModel>> GetStatusesAsync() => await _serversClient.GetStatusesAsync();
    }
}
