using CarPark.Models;

namespace CarPark.APIModels;

public class EnterpriseAPIModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public EnterpriseAPIModel(Enterprise enterprise)
    {
        Id = enterprise.Id;
        Name = enterprise.Name;
        City = enterprise.City;
    }
}
