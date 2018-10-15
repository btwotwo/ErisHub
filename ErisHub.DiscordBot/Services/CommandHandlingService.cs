using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace ErisHub.DiscordBot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discord;
        private IServiceProvider _provider;

        public CommandHandlingService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands, IConfiguration config)
        {
            _commands = commands;
            _config = config;
            _discord = discord;
            _provider = provider;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

            _discord.MessageReceived += MessageReceived;
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;

            if (!(message.HasMentionPrefix(_discord.CurrentUser, ref argPos))) return;

            var context = new SocketCommandContext(_discord, message);
            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            if (result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand)
            {
                await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}
