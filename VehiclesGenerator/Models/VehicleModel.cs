namespace CarParkGenerator.Models;

public class VehicleModel
{
    public int EnterpriseId { get; set; }
    public int BrandId { get; set; }
    public int? ActiveDriverId { get; set; }
    public string RegistrationNumber { get; set; }
    public int Mileage { get; set; }
    public int Price { get; set; }
    public int Year { get; set; }
}
