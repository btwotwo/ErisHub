using System.ComponentModel.DataAnnotations;

namespace ErisHub.DiscordBot.Database.Models.Watcher
{
    public class WatcherSettings
    {
        [Key]
        public string Server { get; set; }

        [Required]
        public ulong MentionRoleId { get; set; }

        [Required]
        public ulong ChannelId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public bool Watching { get; set; }
    }
}
