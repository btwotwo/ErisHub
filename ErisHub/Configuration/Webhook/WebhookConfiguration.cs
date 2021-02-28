using System;

namespace ErisHub.Configuration.Webhook
{
    public class WebhookConfiguration
    {
        public RestartWebhook RestartWebhook { get; set; }
        public Uri LobbyAlert { get; set; }
        public Uri AdminHelp { get; set; }

        public Uri AdminAlert { get; set; }
        public Uri CoderAlert { get; set; }
        public Uri AdmiraltyAlert { get; set; }
    }
}
