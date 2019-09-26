using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ErisHub.DiscordBot.ApiClient;
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
        private readonly ServersApiClient _api;
        public WatcherModule(BaseSocketClient discordClient, BotContext context, ServersApiClient api)
        {
            _discordClient = discordClient;
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44303/webhookHub")
                .Build();

            _db = context;
            _api = api;
            _connection.On<string>(WebhookEvents.ServerRestart, async (serverName) =>
            {
                await ReplyAsync($"Server {serverName} restarted!");
            });
        }



        [Command("servers")]
        public async Task GetServers()
        {
            var servers = await GetServerNamesAsync();

            await ReplyAsync($"Available servers: {string.Join("\n", servers)}.");
        }
        
        [Command("settings")]
        public async Task UpdateSettings(string serverName, IRole mentionRole, IChannel mentionChannel, string mentionMessage)
        {

            var (valid, messageChannel) = await ValidateSettings(serverName, mentionChannel, mentionMessage);

            if (!valid)
            {
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
            settings.Watching = true;

            await _db.SaveChangesAsync();

            await ReplyAsync("Settings updated.");
        }

        [Command("start")]
        public async Task StartWatching(string server)
        {
            var settings = _db.WatcherSettings.SingleOrDefaultAsync(x => x.Server == server);

            if (settings == null)
            {
                await ReplyAsync(@"Watcher settings for ""{server}"" are not found. Please create them first with `settings serverName, mentionRole, mentionChannel, mentionMessage`");
                return;
            }

            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }

            await ReplyAsync("Started.");
        }

        [Command("stop")]
        public async Task StopWatching(string server)
        {
            var serverValid = await ValidateServer(server);

            if (!serverValid)
            {
                return;
            }

            var settings = await _db.WatcherSettings.SingleOrDefaultAsync(x => x.Server == server);

            if (settings == null)
            {
                await ReplyAsync(@"Watcher settings for ""{server}"" are not found. Please create them first with `settings serverName, mentionRole, mentionChannel, mentionMessage`");
                return;
            }

            settings.Watching = false;

            await _db.SaveChangesAsync();

            await ReplyAsync($@"Stopped watching for ""{server}""");
        }

        private async Task<(bool, IMessageChannel)> ValidateSettings(string serverName, IChannel mentionChannel, string mentionMessage)
        {
            var serverValid = await ValidateServer(serverName);

            if (!serverValid)
            {
                return (false, null);
            }

            var messageChannel = await Context.Guild.GetMessageChannelOrDefaultAsync(mentionChannel);

            if (messageChannel == null)
            {
                await ReplyAsync("Invalid channel.");
                return (false, null);
            }

            if (string.IsNullOrWhiteSpace(mentionMessage))
            {
                await ReplyAsync("Please provide a mention message.");
                return (false, null);
            }

            return (true, messageChannel);
        }

        private async Task<bool> ValidateServer(string serverName)
        {
            var servers = await GetServerNamesAsync();

            if (!servers.Contains(serverName))
            {
                await ReplyAsync($"Server name is not valid. Available servers: : {string.Join("\n", servers)}.");

                return false;
            }

            return true;
        }

        private async Task<List<string>> GetServerNamesAsync() => (await _api.GetAllServersAsync()).Select(x => x.Name).ToList();
        
    }
}