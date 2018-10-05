using System.Collections.Generic;

namespace ErisHub.Database.Models
{
    public class PollOption
    {
        public PollOption()
        {
            PollVotes = new HashSet<PollVote>();
        }

        public int Id { get; set; }
        public int PollId { get; set; }
        public string Text { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        public Poll Poll { get; set; }
        public ICollection<PollVote> PollVotes { get; set; }
    }
}
