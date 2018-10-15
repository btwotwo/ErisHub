using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErisHub.Configuration;
using ErisHub.Shared.Server.Models;
using Microsoft.Extensions.Configuration;

namespace ErisHub.Helpers
{
    public class ServerStore
    {
        private readonly IEnumerable<Server> _servers;

        public ServerStore(IConfiguration config)
        {
            _servers = new List<Server>();
            config.GetSection("Servers").Bind(_servers);
        }

        public Server GetServer(string id)
        {
            return _servers.SingleOrDefault(x => x.Id == id);
        }

        public IEnumerable<Server> GetAllServers()
        {
            return _servers;
        }
    }
}
