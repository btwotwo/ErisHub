using System;
using System.Net.Http;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using EFCore.DbContextFactory.Extensions;
using ErisHub.DiscordBot.ApiClient;
using ErisHub.DiscordBot.Database;
using ErisHub.DiscordBot.Database.Models.Newcomer;
using ErisHub.DiscordBot.Modules.Newcomer;
using ErisHub.DiscordBot.Modules.Server;
using ErisHub.DiscordBot.Services;
using ErisHub.DiscordBot.Util.CachedDbEntity;
using ErisHub.DiscordBot.Util.CachedRepo;
using ErisHub.DiscordBot.Util.Timer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

// todo: bot can't access hub
// check if the status update is working

namespace ErisHub.DiscordBot
{
    internal static class ServiceProviderExtensions
    {
        public static ServiceCollection AddBaseServices(this ServiceCollection provider, Config config, DiscordSocketClient client, string connectionString)
        {
            var apiHttpClient = new HttpClient()
            {
                BaseAddress = new Uri(config.HubApiUrl)
            };

            provider.AddSingleton(config)
                .AddSingleton<IWaitingTimerProvider, WaitingTimerProvider>()
                .AddSingleton<BaseSocketClient>(client)
                .AddSingleton<IDiscordClient>(client)
                .AddDbContext<BotContext>(builder => builder.UseSqlite(connectionString))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<InteractiveService>()
                .AddSingleton(apiHttpClient);


            provider.AddDbContextFactory<BotContext>(o => o.UseSqlite(connectionString));

            return provider;
        }

        public static IServiceCollection AddApiClients(this IServiceCollection provider)
        {
            return provider
                .AddSingleton<BansApiClient>()
                .AddSingleton<PlayersApiClient>()
                .AddSingleton<ServersClient>();
        }

        public static IServiceCollection AddCachedRepos(this IServiceCollection provider)
        {
            return provider
                .AddSingleton<ICachedRepo<NewcomerSetting>, CachedRepo<NewcomerSetting>>()
                .AddSingleton<ICachedDbEntity<NewcomerConfig>, CachedDbEntity<NewcomerConfig>>();
        }

        public static IServiceCollection AddUserServices(this IServiceCollection provider)
        {
            return provider
                .AddSingleton<IStatusUpdater, StatusUpdater>()
                .AddSingleton<IStatusProvider, StatusProvider>()
                .AddSingleton<IStatusMessageFactory, StatusMessageFactory>()
                .AddSingleton<IStatusHider, StatusHider>()
                .AddSingleton<NewcomerHandler>();
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection provider, IConfiguration config)
        {
            var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            return provider
                .AddLogging(builder => 
                {
                    builder.AddSerilog(logger);
                })
                .AddSingleton<LoggingService>()
                .AddSingleton(config);
        }



    }
}
