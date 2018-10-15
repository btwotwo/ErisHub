using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ErisHub.DiscordBot.Helpers;
using Microsoft.Extensions.Configuration;

namespace ErisHub.DiscordBot.Services.Status
{
    public class StatusService
    {
        private Timer _statusUpdateTimer;
        private readonly ServerStore _servers;
        private IUserMessage _statusMessage;
        private readonly DiscordSocketClient _discord;
        private readonly IConfiguration _config;


        public StatusService(ServerStore store, DiscordSocketClient discord, IConfiguration config)
        {
            _servers = store;
            _config = config;
            _discord = discord;
        }

        public async Task InitAsync()
        {
            var initialEmbedBuilder = new EmbedBuilder();
            initialEmbedBuilder.WithColor(Color.Green);

            foreach (var server in _servers.Servers)
            {
                if (server.Online)
                {
                    initialEmbedBuilder.AddField(server.Name, $"<{server.Address}>");
                    initialEmbedBuilder.AddInlineField("Players", server.Players);
                    initialEmbedBuilder.AddInlineField("Admins", server.Admins);
                }
                else
                {
                    initialEmbedBuilder.AddField(server.Name, "Server is offline");
                }

                initialEmbedBuilder.AddField("\u200b", "\u200b"); // to emulate a new line
            }

            var channel = (IMessageChannel)_discord.GetChannel(_config.GetValue<ulong>("StatusChannelId"));
            var message = await channel.SendMessageAsync("", false, initialEmbedBuilder.Build());
            _statusMessage = message;

        }

        private async void UpdateStatusesAsync(object state)
        {
        }
    }
}
