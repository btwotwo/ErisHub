namespace ErisHub.DiscordBot.Database.Models.Newcomer
{
    public class NewcomerConfig: IDbModel
    {
        public int Id { get; set; }

        public ulong? NewcomerRoleId { get; set; }
    }
}
