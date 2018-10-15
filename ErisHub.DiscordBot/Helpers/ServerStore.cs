using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ErisHub.DiscordBot.Services.Status;
using ErisHub.Shared.Server.Models;
using Newtonsoft.Json;

namespace ErisHub.DiscordBot.Helpers
{
    public class ServerStore
    {
        private readonly HttpClient _http;
        public IEnumerable<StatusModel> Servers { get; private set; }

        public ServerStore(HttpClient http)
        {
            _http = http;
        }

        public async Task InitAsync()
        {
            var response = await _http.GetAsync("api/servers");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve server list.");
            }

            var servers =
                JsonConvert.DeserializeObject<IEnumerable<StatusModel>>(await response.Content.ReadAsStringAsync());

            Servers = servers;
        }
    }
}
