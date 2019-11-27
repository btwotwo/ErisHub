using System;
using System.Net;
using System.Threading.Tasks;
using ErisHub.Helpers.Auth;
using ErisHub.Shared.SingalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ErisHub.Core.Webhook
{
    [Authorize(AuthenticationSchemes = Schemes.ApiKeyScheme)]
    [Route("webhook")]
    public class WebhookController : ApiController
    {
        private readonly IHubContext<WebhookHub> _hub;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IHubContext<WebhookHub> hub, ILogger<WebhookController> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        [HttpGet("{serverName}/restart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RestartHook(string serverName)
        {
            var id = Guid.NewGuid().ToString();
            _logger.LogInformation($"Sending ServerRestart event {id}");
            await _hub.Clients.All.SendAsync(WebhookEvents.ServerRestart, serverName, id);
            _logger.LogInformation($"Sent ServerRestart event {id}");
            return Ok();
        }

    }
}