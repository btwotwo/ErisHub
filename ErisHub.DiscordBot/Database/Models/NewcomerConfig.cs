using System;
using System.Collections.Generic;
using System.Text;
using ErisHub.DiscordBot.Util;

namespace ErisHub.DiscordBot.Database.Models
{
    public class NewcomerConfig
    {
        public int Id { get; set; }

        public ulong? NewcomerRoleId { get; set; }
    }
}
