using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErisHub.Configuration
{
    public class Server
    {
        public string Name { get; set; }
        public string ConfigPath { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
