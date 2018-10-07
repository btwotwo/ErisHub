using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErisHub.Core.Players
{
    public class PlayerDto
    {
        public int Id { get; set; }

        public string Ckey { get; set; }

        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }

        public int Flags { get; set; }
        public string Rank { get; set; }

        public string Ip { get; set; }
        public string Cid { get; set; }
    }
}
