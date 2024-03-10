using CarPark.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarPark.ViewModels.Vehicle;

public class VehicleViewModel
{
    public int Id { get; set; }
    public int? ActiveDriverId { get; set; }
    public int BrandId { get; set; }
    public int EnterpriseId { get; set; }
    public string RegistrationNumber { get; set; }
    public DateTimeOffset PurchaceDateTimeOffset { get; set; }
    public int Mileage { get; set; }
    public int Price { get; set; }
    public int Year { get; set; }
    public List<int> DriversIds { get; set; } = new();
    public SelectList? BrandsSelectList { get; set; }
    public SelectList? EnterprisesSelectList { get; set; }
    public MultiSelectList? DriversSelectList { get; set; }
    public SelectList? ActiveDriverSelectList { get; set; }
    public VehicleViewModel() { }
    public VehicleViewModel(Models.Vehicle vehicle)
    {
        Id = vehicle.Id;
        ActiveDriverId = vehicle.ActiveDriverId;
        BrandId = vehicle.BrandId;
        EnterpriseId = vehicle.EnterpriseId;
        RegistrationNumber = vehicle.RegistrationNumber;
        PurchaceDateTimeOffset = vehicle.PurchaceDateTimeOffset;
        Mileage = vehicle.Mileage;
        Price = vehicle.Price;
        Year = vehicle.Year;
        DriversIds = vehicle.DriversVehicles.Select(dv => dv.DriverId).ToList();
    }
    public void AddSelectLists(ApplicationDbContext dbContext, ClaimsPrincipal claimsPrincipal)
    {
        var userEnterprises = Enterprise.GetUserEnterprises(dbContext, claimsPrincipal);

        var driversWithSameEnterprise = dbContext.Drivers
            .Where(v => v.EnterpriseId == EnterpriseId);

        var driversAttachedToVehicle = dbContext.DriversVehicles
            .Where(dv => dv.VehicleId == Id)
            .Include(dv => dv.Driver)
            .Select(dv => dv.Driver);

        EnterprisesSelectList = new SelectList(userEnterprises, "Id", "Name", EnterpriseId);
        DriversSelectList = new MultiSelectList(driversWithSameEnterprise, "Id", "Name");
        ActiveDriverSelectList = new SelectList(driversAttachedToVehicle, "Id", "Name", ActiveDriverId);
        BrandsSelectList = new SelectList(dbContext.Brands, "Id", "Name", BrandId);
    }
}
