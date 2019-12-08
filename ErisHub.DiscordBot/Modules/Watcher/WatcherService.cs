using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ErisHub.DiscordBot.Database;
using ErisHub.Shared.SingalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ErisHub.DiscordBot.Modules.Watcher
{
    public class WatcherService
    {
        private HubConnection _connection;
        private readonly Func<BotContext> _getDb;
        private readonly ILogger<WatcherService> _logger;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores;
        private readonly IConfiguration _config;
        private bool _initialized = false;

        private readonly BaseSocketClient _discordClient;


        public WatcherService(Func<BotContext> contextFactory, ILogger<WatcherService> logger, IConfiguration config, BaseSocketClient discord)
        {
            _getDb = contextFactory;
            _logger = logger;
            _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
            _config = config;
            _discordClient = discord;
        }

        public async Task InitAsync()
        {
            if (_initialized)
                return;

            var hubApiUrl = new Uri(_config["HubApiUrl"]);
            Uri.TryCreate(hubApiUrl, "/webhookHub", out var webhookUrl);
            _connection = new HubConnectionBuilder()
                .WithUrl(webhookUrl)
                .Build();
            await _connection.StartAsync();

            _initialized = true;
            _connection.On<string, string>(WebhookEvents.ServerRestart, async (serverName, guid) => await HandleWebhookNotification(serverName, guid));
        }

        private async Task HandleWebhookNotification(string serverName, string guid)
        {
            _logger.LogInformation($"Got {serverName} with id {guid}");

            using (var db = _getDb())
            {
                var serverExists = await db.WatcherSettings.AnyAsync(x => x.Server == serverName);

                if (!serverExists)
                {
                    return;
                }

                //var semaphore = _semaphores.GetOrAdd(serverName, new SemaphoreSlim(1, 1));

                try
                {
                    //await semaphore.WaitAsync();

                    var server = await db.WatcherSettings.SingleAsync(x => x.Server == serverName);

                    var shouldNotify = server.Watching; //&& DateTime.Now - server.LastRestart.GetValueOrDefault() > TimeSpan.FromMinutes(2);

                    if (shouldNotify)
                    {
                        var channel = _discordClient.GetChannel(server.ChannelId) as IMessageChannel;
                        var guild = (channel as IGuildChannel).Guild;
                        var role = guild.GetRole(server.MentionRoleId);
                        server.LastRestart = DateTime.Now;

                        await db.SaveChangesAsync();

                        await channel.SendMessageAsync($"{role.Mention} {server.Message}");
                    }
                }
                finally
                {
                    //semaphore.Release();
                }
            }
        }
    }
}
