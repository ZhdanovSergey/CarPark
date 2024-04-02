using CarPark.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarPark.ViewModels.Vehicle;

public class VehicleDetailsViewModel
{
    public int Id { get; set; }
    public Driver? ActiveDriver { get; set; }
    public Brand Brand { get; set; }
    public Enterprise Enterprise { get; set; }
    public Pagination<Ride> RidesPagination { get; init; }
    public string RegistrationNumber { get; set; }
    public DateTimeOffset PurchaceDateTimeOffset { get; set; }
    public int Mileage { get; set; }
    public int Price { get; set; }
    public int Year { get; set; }
    public List<Driver> Drivers { get; set; } = new();
    public VehicleDetailsViewModel() { }
    public VehicleDetailsViewModel(Models.Vehicle vehicle)
    {
        if (vehicle.Brand is null)
            throw new ArgumentNullException(nameof(vehicle.Brand));

        if (vehicle.DriversVehicles is null)
            throw new ArgumentNullException(nameof(vehicle.DriversVehicles));

        if (vehicle.DriversVehicles.Any(dv => dv.Driver is null))
            throw new ArgumentNullException($"Driver in {nameof(vehicle.DriversVehicles)}");

        if (vehicle.Enterprise is null)
            throw new ArgumentNullException(nameof(vehicle.Enterprise));

        Id = vehicle.Id;
        ActiveDriver = vehicle.ActiveDriver;
        Brand = vehicle.Brand;
        Enterprise = vehicle.Enterprise;
        RegistrationNumber = vehicle.RegistrationNumber;
        PurchaceDateTimeOffset = vehicle.PurchaceDateTimeOffset;
        Mileage = vehicle.Mileage;
        Price = vehicle.Price;
        Year = vehicle.Year;
        Drivers = vehicle.DriversVehicles.Select(dv => dv.Driver).ToList();
    }
}
