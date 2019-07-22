using System.Net;
using System.Threading.Tasks;
using ErisHub.Helpers.Auth;
using ErisHub.Shared.SingalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ErisHub.Core.Webhook
{
    [Authorize(AuthenticationSchemes = Schemes.ApiKeyScheme)]
    public class WebhookController : ApiController
    {
        private readonly IHubContext<WebhookHub> _hub;

        public WebhookController(IHubContext<WebhookHub> hub)
        {
            _hub = hub;
        }

        [HttpGet("{serverName}/restarted")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RestartHook(string serverName)
        {
            await _hub.Clients.All.SendAsync(WebhookEvents.ServerRestart, serverName);
            return Ok();
        }

    }
}