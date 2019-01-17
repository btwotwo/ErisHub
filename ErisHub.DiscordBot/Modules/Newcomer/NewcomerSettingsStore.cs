using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErisHub.DiscordBot.Database;
using ErisHub.DiscordBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.DiscordBot.Modules.Newcomer
{
    public class NewcomerSettingsStore
    {
        private readonly object _locker = new object();

        private IReadOnlyCollection<NewcomerSetting> _settings;
        private NewcomerConfig _config;

        private readonly BotContext _db;

        public IReadOnlyCollection<NewcomerSetting> Settings
        {
            get
            {
                lock (_locker)
                {
                    return _settings;
                }
            }
        }

        public NewcomerConfig Config
        {
            get
            {
                lock (_locker)
                {
                    return _config;
                }
            }
        }

        public NewcomerSettingsStore(BotContext db)
        {
            _db = db;

            UpdateSettings();
            UpdateConfig();
        }

        public void UpdateSettings()
        {
            lock (_locker)
            {
                _settings = _db.NewcomerSettings.ToList();
            }
        }

        public void UpdateConfig()
        {
            lock (_locker)
            {
                _config = _db.NewcomerConfigs.Single();
            }
        }


    }
}
