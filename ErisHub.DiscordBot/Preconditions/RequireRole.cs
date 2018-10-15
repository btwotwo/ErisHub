using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErisHub.DiscordBot.Preconditions
{
    public class RequireRoleAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = services.GetService<IConfiguration>();

            var ids = config.GetValue<ulong[]>("AllowedRoles");

            var user = context.User as SocketGuildUser;
            var allowed = user?.Roles.Any(role => ids.Contains(role.Id));

            return allowed.HasValue && allowed.Value
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError("You do not have permissions to do that."));
        }
    }
}
