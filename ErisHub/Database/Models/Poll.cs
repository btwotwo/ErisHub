using System;
using System.Collections.Generic;

namespace ErisHub.Database.Models
{
    public class Poll
    {
        public Poll()
        {
            PollOptions = new HashSet<PollOption>();
            PollTextReplies = new HashSet<PollTextReply>();
            PollVotes = new HashSet<PollVote>();
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Question { get; set; }

        public ICollection<PollOption> PollOptions { get; set; }
        public ICollection<PollTextReply> PollTextReplies { get; set; }
        public ICollection<PollVote> PollVotes { get; set; }
    }
}
