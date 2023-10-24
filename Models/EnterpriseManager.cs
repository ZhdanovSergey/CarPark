namespace CarPark.Models
{
    public class EnterpriseManager
    {
        public int EnterpriseId { get; set; }
        public int ManagerId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public Manager? Manager { get; set; }
    }
}
