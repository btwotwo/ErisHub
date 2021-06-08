using System;

namespace ErisHub.Configuration.Webhook
{
    public interface IWebhookConfig
    {
        Uri Url { get; set; }
    }
}
