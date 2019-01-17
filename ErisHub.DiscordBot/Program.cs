using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ErisHub.DiscordBot.Database;
using ErisHub.DiscordBot.Database.Models;
using ErisHub.DiscordBot.Modules.Newcomer;
using ErisHub.DiscordBot.Modules.Server;
using ErisHub.DiscordBot.Services;
using ErisHub.DiscordBot.Util.CachedDbEntity;
using ErisHub.DiscordBot.Util.CachedRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ErisHub.DiscordBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }
        

        private DiscordSocketClient _client;
        private IConfiguration _config;
        private Config _configObject;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Ready += NotifyRestart;

            _config = BuildConfig();

            _configObject = new Config();
            _config.Bind(_configObject);

            var services = ConfigureServices();

            services.GetRequiredService<LoggingService>().Init();

            await _client.LoginAsync(TokenType.Bot, _configObject.Token);
            await _client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);
            services.GetRequiredService<NewcomerHandler>().Init();

            await Task.Delay(-1);

        }

        private async Task NotifyRestart()
        {
            var channelId = _configObject.BotSettings.BotStatusChannelId;

            var channel = (IMessageChannel) _client.GetChannel(channelId);

            await channel.SendMessageAsync($"Bot has been restarted.");
        }

        private IServiceProvider ConfigureServices()
        {
            var apiHttpClient = new HttpClient()
            {
                BaseAddress = new Uri(_configObject.HubApiUrl)
            };
            return new ServiceCollection()
                // Base
                .AddSingleton(_configObject)
                .AddSingleton<BaseSocketClient>(_client)
                .AddDbContext<BotContext>(builder => builder.UseSqlite(_config.GetConnectionString("Bot")))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<ICachedRepo<NewcomerSetting> ,CachedRepo<NewcomerSetting>>()
                .AddSingleton<ICachedDbEntity<NewcomerConfig>, CachedDbEntity<NewcomerConfig>>()
                // Logging
                .AddLogging(builder => builder.AddConsole())
                .AddSingleton<LoggingService>()
                .AddSingleton(apiHttpClient)
                .AddSingleton<StatusService>()
                .AddSingleton<NewcomerHandler>()
                //Extra
                .AddSingleton(_config)
                .BuildServiceProvider();
        }

        private static IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile("config.dev.json")
#else
                .AddJsonFile("config.json")
#endif
                .Build();
        }
    }
}
