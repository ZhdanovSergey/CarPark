﻿using CarPark.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace CarPark.ViewModels
{
    public class VehicleViewModel
    {
        public int Id { get; set; }
        public int? ActiveDriverId { get; set; }
        public int BrandId { get; set; }
        public int EnterpriseId { get; set; }
        public string RegistrationNumber { get; set; }
        public int Mileage { get; set; }
        public int Price { get; set; }
        public int Year { get; set; }
        public List<int> DriversIds { get; set; } = new();
        public SelectList? BrandsSelectList { get; set; }
        public SelectList? EnterprisesSelectList { get; set; }
        public MultiSelectList? DriversSelectList { get; set; }
        public SelectList? ActiveDriverSelectList { get; set; }
        public VehicleViewModel() { }
        public VehicleViewModel(Vehicle vehicle)
        {
            Id = vehicle.Id;
            ActiveDriverId = vehicle.ActiveDriverId;
            BrandId = vehicle.BrandId;
            EnterpriseId = vehicle.EnterpriseId;
            RegistrationNumber = vehicle.RegistrationNumber;
            Mileage = vehicle.Mileage;
            Price = vehicle.Price;
            Year = vehicle.Year;
            DriversIds = vehicle.DriversVehicles.Select(dv => dv.DriverId).ToList();
        }
        public async Task AddCreateSelectLists
        (
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            ClaimsPrincipal claimsPrincipal
        )
        {
            var userEnterprises = await Enterprise.GetUserEnterprises(dbContext, userManager, claimsPrincipal);
            this.EnterprisesSelectList = new SelectList(await userEnterprises.ToListAsync(), "Id", "Name", this.EnterpriseId);
            this.BrandsSelectList = new SelectList(await dbContext.Brands.ToListAsync(), "Id", "Name", this.BrandId);
        }
        public async Task AddEditSelectLists
        (
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            ClaimsPrincipal claimsPrincipal
        )
        {
            var userEnterprises = await Enterprise.GetUserEnterprises(dbContext, userManager, claimsPrincipal);

            var driversWithSameEnterprise = await dbContext.Drivers
                .Where(v => v.EnterpriseId == this.EnterpriseId)
                .ToListAsync();

            var driversAttachedToVehicle = await dbContext.DriversVehicles
                .Where(dv => dv.VehicleId == this.Id)
                .Include(dv => dv.Driver)
                .Select(dv => dv.Driver)
                .ToListAsync();

            this.EnterprisesSelectList = new SelectList(await userEnterprises.ToListAsync(), "Id", "Name", this.EnterpriseId);
            this.DriversSelectList = new MultiSelectList(driversWithSameEnterprise, "Id", "Name");
            this.ActiveDriverSelectList = new SelectList(driversAttachedToVehicle, "Id", "Name", this.ActiveDriverId);
            this.BrandsSelectList = new SelectList(await dbContext.Brands.ToListAsync(), "Id", "Name", this.BrandId);
        }
    }
}
