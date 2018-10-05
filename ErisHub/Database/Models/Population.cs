using System;
using System.Collections.Generic;

namespace ErisHub.Database.Models
{
    public class Population
    {
        public int Id { get; set; }
        public int PlayerCount { get; set; }
        public int AdminCount { get; set; }
        public DateTime Time { get; set; }
        public string Server { get; set; }
    }
}
