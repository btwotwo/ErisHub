using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErisHub.Core.Server.Models
{
    public class StatusModel
    {
        public bool Online { get; set; }
        public int Players { get; set; }
        public int Admins { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
