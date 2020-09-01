using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErisHub.Core.Webhook
{
    interface IMentionWebhook
    {
        Task MentionAsync(ulong roleId, string message);
    }
}
