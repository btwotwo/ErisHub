using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ErisHub.DiscordBot.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.dev.json")
                .Build();
            var builder = new DbContextOptionsBuilder<BotContext>();
            var connectionString = configuration.GetConnectionString("Bot");

            builder.UseSqlite(connectionString);
            return new BotContext(builder.Options);
        }
    }
}
