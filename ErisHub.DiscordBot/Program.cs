using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ErisHub.DiscordBot.Services;
using ErisHub.DiscordBot.Services.Status;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _config = BuildConfig();

            var services = ConfigureServices();

            services.GetRequiredService<LoggingService>().Init();

            await _client.LoginAsync(TokenType.Bot, _config["Token"]);
            await _client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            var apiHttpClient = new HttpClient()
            {
                BaseAddress = new Uri(_config["HubApiUrl"])
            };
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                // Logging
                .AddLogging()
                .AddSingleton<LoggingService>()
                .AddSingleton(apiHttpClient)
                .AddSingleton<StatusService>()
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
