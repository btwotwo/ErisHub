using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ErisHub.Helpers.Auth
{
    public class ApiKeyOptions : AuthenticationSchemeOptions
    {
        public string ApiKey { get; set; }
    }

    public class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
    {
        public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var key = Request.Query["key"];

            if (string.IsNullOrWhiteSpace(key))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (key != Options.ApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Wrong key"));
            }

            var identity = new ClaimsIdentity(Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));

        }
    }
}