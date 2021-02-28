using System;

namespace ErisHub.Configuration.Webhook
{
    public class RestartWebhook : IWebhookConfig
    {
        public Uri Url { get; set; }
        public string Message { get; set; }
    }
}
