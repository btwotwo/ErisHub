using System;

namespace ErisHub.Core.Server
{
    public class StatusModel
    {
        public bool Online { get; set; }
        public int Players { get; set; }
        public int Admins { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string RoundDuration { get; set; }
    }
}
