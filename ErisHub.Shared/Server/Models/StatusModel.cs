namespace ErisHub.Shared.Server.Models
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
