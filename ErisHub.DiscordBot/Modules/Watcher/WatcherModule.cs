using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ErisHub.DiscordBot.Database;
using ErisHub.DiscordBot.Database.Models.Watcher;
using ErisHub.DiscordBot.Util;
using ErisHub.Shared.SingalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.DiscordBot.Modules.Watcher
{
    [Group("watcher")]
    public class WatcherModule : ModuleBase
    {
        private readonly BaseSocketClient _discordClient;
        private readonly HubConnection _connection;
        private readonly BotContext _db;

        public WatcherModule(BaseSocketClient discordClient, BotContext context)
        {
            _discordClient = discordClient;
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44303/webhookHub")
                .Build();

            _db = context;
        }

        [Command("settings")]
        public async Task UpdateSettings(string serverName, IRole mentionRole, IChannel mentionChannel, string mentionMessage)
        {
            serverName = serverName.ToLower();

            var messageChannel = await Context.Guild.GetMessageChannelOrDefaultAsync(mentionChannel);

            if (messageChannel == null)
            {
                await ReplyAsync("Invalid channel.");
                return;
            }

            if (string.IsNullOrWhiteSpace(mentionMessage))
            {
                await ReplyAsync("Please provide a mention message.");
                return;
            }


            var settings = await _db.WatcherSettings.SingleOrDefaultAsync(x => x.Server == serverName);

            if (settings == null)
            {
                settings = new WatcherSettings()
                {
                    Server = serverName
                };

                _db.WatcherSettings.Add(settings);
            }


            settings.ChannelId = messageChannel.Id;
            settings.Message = mentionMessage;
            settings.MentionRoleId = mentionRole.Id;

            await _db.SaveChangesAsync();

            await ReplyAsync("Settings updated.");
        }

        [Command("start")]
        public async Task StartWatching(string server)
        {
            var settings = _db.WatcherSettings.SingleOrDefaultAsync(x => x.Server == server);

            if (settings == null)
            {
                await ReplyAsync($@"Settings for server ""{server}"" are not found.");
                return;
            }

            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }

            _connection.On<string>(WebhookEvents.ServerRestart, async (serverName) =>
            {
                await ReplyAsync($"Server {serverName} restarted!");
            });

            await ReplyAsync("Started.");
        }

        [Command("stop")]
        public async Task StopWatching()
        {
            _connection.Remove(WebhookEvents.ServerRestart);

            await ReplyAsync("Stopped.");
        }
    }
}
