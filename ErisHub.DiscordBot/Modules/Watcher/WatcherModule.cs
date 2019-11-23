using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using Microsoft.Extensions.Configuration;

namespace ErisHub.DiscordBot.Modules.Watcher
{
    [Group("watcher")]
    public class WatcherModule : ModuleBase
    {
        private readonly BaseSocketClient _discordClient;
        private readonly HubConnection _connection;
        private readonly Func<BotContext> _dbFactory;
        private readonly BotContext _db;
        private readonly ServersClient _api;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores;
        public WatcherModule(BaseSocketClient discordClient,  ServersClient api, IConfiguration config, Func<BotContext> factory, BotContext db)
        {
            var hubApiUrl = new Uri(config["HubApiUrl"]);
            Uri.TryCreate(hubApiUrl, "/webhookHub", out var webhookUrl);
            _discordClient = discordClient;
            _connection = new HubConnectionBuilder()
                .WithUrl(webhookUrl)
                .Build();

            _dbFactory = factory;
            _db = db;
            _api = api;
            _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
            _connection.StartAsync().Wait();
            _connection.On<string>(WebhookEvents.ServerRestart, async (serverName) => await HandleWebhookNotification(serverName));
        }


        [Command("get-settings")]
        public async Task GetSettings()
        {
            var settings = await _db.WatcherSettings.ToListAsync();

            foreach (var setting in settings)
            {
                await ReplyAsync($@"Server: ``{setting.Server}``
Mention Role: ``{Context.Guild.GetRole(setting.MentionRoleId).Name}``
Mention Channel: ``{(await Context.Guild.GetChannelAsync(setting.ChannelId)).Name}``
Message: ``{setting.Message}``
");
            }


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
            var settings = await _db.WatcherSettings.SingleOrDefaultAsync(x => x.Server == server);

            if (settings == null)
            {
                await ReplyAsync(@"Watcher settings for ""{server}"" are not found. Please create them first with `settings serverName, mentionRole, mentionChannel, mentionMessage`");
                return;
            }

            settings.Watching = true;
            await _db.SaveChangesAsync();
            await ReplyAsync($"Started watching for ``{settings.Server}``");
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

        private async Task HandleWebhookNotification(string serverName)
        {
            using(var db = _dbFactory())
            {
                var serverExists = await db.WatcherSettings.AnyAsync(x => x.Server == serverName);

                if (!serverExists)
                {
                    return;
                }

                var semaphore = _semaphores.GetOrAdd(serverName, new SemaphoreSlim(1, 1));

                try
                {
                    await semaphore.WaitAsync();

                    var server = await db.WatcherSettings.SingleAsync(x => x.Server == serverName);

                    var shouldNotify = server.Watching && DateTime.Now - server.LastRestart.GetValueOrDefault() > TimeSpan.FromSeconds(30);

                    if (shouldNotify)
                    {
                        var channel = _discordClient.GetChannel(server.ChannelId) as IMessageChannel;
                        var guild = (channel as IGuildChannel).Guild;
                        var role = guild.GetRole(server.MentionRoleId);
                        server.LastRestart = DateTime.Now;

                        await db.SaveChangesAsync();

                        await Task.Delay(TimeSpan.FromSeconds(10));

                        await channel.SendMessageAsync($"{role.Mention} {server.Message}");
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }

        }
        
    }
}