using ErisHub.Configuration.Webhook;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErisHub.Configuration
{
    public class Server
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConfigPath { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public WebhookConfiguration WebhookConfiguration { get; set; }
    }
}
