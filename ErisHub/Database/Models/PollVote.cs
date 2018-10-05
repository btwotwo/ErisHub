using System;

namespace ErisHub.Database.Models
{
    public class PollVote
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int PollId { get; set; }
        public int PlayerId { get; set; }
        public int OptionId { get; set; }

        public PollOption Option { get; set; }
        public Player Player { get; set; }
        public Poll Poll { get; set; }
    }
}
