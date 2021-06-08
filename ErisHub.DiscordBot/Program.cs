using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ErisHub.DiscordBot.Database;
using ErisHub.DiscordBot.Modules.Newcomer;
using ErisHub.DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

// todo: bot can't access hub
// check if the status update is working

namespace ErisHub.DiscordBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await new Program().MainAsync();
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

            await services.GetService<BotContext>().Database.MigrateAsync();

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

            var channel = (IMessageChannel)_client.GetChannel(channelId);

            await channel.SendMessageAsync($"Bot has been restarted.");
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddBaseServices(_configObject, _client, _config.GetConnectionString("Bot"))
                .AddApiClients()
                .AddCachedRepos()
                .AddInfrastructure(_config)
                .AddUserServices()
                .BuildServiceProvider();
        }

        private static IConfiguration BuildConfig()
        {
            var env = new HostingEnvironment()
            {
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootFileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory),
                ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
                EnvironmentName = Environment.GetEnvironmentVariable("BOT_ENV")
            };

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("BOT")
                .Build();
        }
    }
}
