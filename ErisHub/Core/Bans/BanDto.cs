using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErisHub.Core.Bans
{
    public class BanDto
    {
        public int Id { get; set; }
        public string Server { get; set; }

        public string Type { get; set; }
        public string Reason { get; set; }
        public string Job { get; set; }

        public int Duration { get; set; }
        public DateTime Time { get; set; }

        public string TargetCkey { get; set; }
        public string BannedByCkey { get; set; }

        public bool? Unbanned { get; set; }
        public DateTime? UnbannedTime { get; set; }
        public string UnbannedByCkey { get; set; }
    }
}
