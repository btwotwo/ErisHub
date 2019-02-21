using System;

namespace ErisHub.Shared.Server.Models
{
    public class StatusModel
    {
        public bool Online { get; set; }
        public int Players { get; set; }
        public int Admins { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string RoundDuration { get; set; }

        public override bool Equals(object obj)
        {
            return obj is StatusModel model &&
                   Online == model.Online &&
                   Players == model.Players &&
                   Admins == model.Admins &&
                   Address == model.Address &&
                   Name == model.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Online, Players, Admins, Address, Name);
        }
    }
}
