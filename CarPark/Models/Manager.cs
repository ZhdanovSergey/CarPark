namespace CarPark.Models
{
    public class Manager : ApplicationUser
    {
        public List<EnterpriseManager> EnterprisesManagers { get; set; } = new();
    }
}
