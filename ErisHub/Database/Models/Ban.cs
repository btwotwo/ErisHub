using System;
using System.Collections.Generic;

namespace ErisHub.Database.Models
{
    public class Ban
    {
        public int Id { get; set; }
        public string Server { get; set; }
        public string Type { get; set; }
        public string Ip { get; set; }
        public string Cid { get; set; }
        public string Reason { get; set; }
        public string Job { get; set; }
        public int Duration { get; set; }
        public DateTime Time { get; set; }
        public int TargetId { get; set; }
        public int BannedById { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool? Unbanned { get; set; }
        public DateTime? UnbannedTime { get; set; }
        public int? UnbannedById { get; set; }

        public Player BannedBy { get; set; }
        public Player Target { get; set; }
        public Player UnbannedBy { get; set; }
    }
}
