using System.ComponentModel.DataAnnotations;

namespace ErisHub.DiscordBot.Database.Models.Newcomer
{
    public class NewcomerSetting: IDbModel
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
