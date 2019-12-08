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
using Microsoft.Extensions.Logging;

namespace ErisHub.DiscordBot.Modules.Watcher
{
    [Group("watcher")]
    public class WatcherModule : ModuleBase
    {
        private readonly BotContext _db;
        private readonly ServersClient _api;
        private readonly ILogger<WatcherModule> _logger;

        public WatcherModule(ServersClient api, BotContext db, ILogger<WatcherModule> logger)
        {
            _db = db;
            _api = api;
            _logger = logger;
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
    }
}
