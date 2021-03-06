﻿using System.Linq;
using System.Threading.Tasks;
using ErisHub.DiscordBot.Database.Models;
using ErisHub.DiscordBot.Database.Models.Newcomer;
using ErisHub.DiscordBot.Database.Models.Watcher;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.DiscordBot.Database
{
    public class BotContext: DbContext
    {
        public BotContext(DbContextOptions options): base(options) { }
    
        public DbSet<NewcomerConfig> NewcomerConfigs { get; set; }
        public DbSet<NewcomerSetting> NewcomerSettings { get; set; }
        public DbSet<WatcherSettings> WatcherSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewcomerConfig>().HasData(new NewcomerConfig()
            {
                Id = 1,
                NewcomerRoleId = null
            });
        }

    }
}
