using System;
using System.Collections.Generic;

namespace ErisHub.Database.Models
{
    public partial class PollTextReply
    {
        public int Id { get; set; }
        public DateTime? Time { get; set; }
        public int? PollId { get; set; }
        public int? PlayerId { get; set; }
        public string Text { get; set; }

        public Player Player { get; set; }
        public Poll Poll { get; set; }
    }
}
