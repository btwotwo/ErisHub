using System;
using System.Collections.Generic;

namespace ErisHub.Database.Models
{
    public class Player
    {
        public Player()
        {
            BansBannedBy = new HashSet<Ban>();
            BansTarget = new HashSet<Ban>();
            BansUnbannedBy = new HashSet<Ban>();
            Books = new HashSet<Book>();
            PollTextReplies = new HashSet<PollTextReply>();
            PollVotes = new HashSet<PollVote>();
        }

        public int Id { get; set; }
        public string Ckey { get; set; }
        public DateTime? Registered { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public string Ip { get; set; }
        public string Cid { get; set; }
        public string Rank { get; set; }
        public int Flags { get; set; }
        public string ByondVersion { get; set; }
        public string Country { get; set; }

        public ICollection<Ban> BansBannedBy { get; set; }
        public ICollection<Ban> BansTarget { get; set; }
        public ICollection<Ban> BansUnbannedBy { get; set; }
        public ICollection<Book> Books { get; set; }
        public ICollection<PollTextReply> PollTextReplies { get; set; }
        public ICollection<PollVote> PollVotes { get; set; }
    }
}
