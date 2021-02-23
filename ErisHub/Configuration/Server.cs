using System;
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

    public interface IWebhookConfig
    {
        Uri Url { get; set; }
    }

    public class RestartWebhookConfig: IWebhookConfig
    {
        public Uri Url { get; set; }
        public ulong RoleId { get; set; }
    }

    public class LobbyAlert: IWebhookConfig
    {
        public Uri Url { get; set; }

    }

    public class WebhookConfiguration
    {
        public RestartWebhookConfig RestartWebhook{ get; set; }
        public Uri LobbyAlert { get; set; }
        public Uri AdminHelp { get; set; }

        public Uri AdminAlert { get; set; }
        public Uri CoderAlert { get; set; }
        public Uri AdmiraltyAlert { get; set; }
    }
}
