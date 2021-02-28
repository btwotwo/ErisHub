using System;

namespace ErisHub.Configuration.Webhook
{
    public class LobbyAlert : IWebhookConfig
    {
        public Uri Url { get; set; }

    }
}
