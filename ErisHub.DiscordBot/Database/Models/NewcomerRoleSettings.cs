using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ErisHub.DiscordBot.Util;

namespace ErisHub.DiscordBot.Database.Models
{
    public class NewcomerSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong ChannelId { get; set; }

        [Required]
        public ulong MessageId { get; set; }

        [Required]
        public ulong RoleId { get; set; }

        [Required]
        public ulong EmoteId { get; set; }
    }
}
