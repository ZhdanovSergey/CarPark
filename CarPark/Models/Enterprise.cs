﻿using CarPark.Migrations;
using CarPark.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarPark.Models;

public class Enterprise
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string TimeZoneId { get; set; } = "UTC";
    public List<Driver> Drivers { get; set; } = new();
    public List<Vehicle> Vehicles { get; set; } = new();
    public List<DriverVehicle> DriversVehicles { get; set; } = new();
    public List<EnterpriseManager> EnterprisesManagers { get; set; } = new();
    public Enterprise() { }
    public Enterprise(EnterpriseViewModel enterpriseVM)
    {
        Id = enterpriseVM.Id;
        Name = enterpriseVM.Name;
        City = enterpriseVM.City;
        TimeZoneId = enterpriseVM.TimeZoneId;
    }
    public static async Task<bool> CheckAccess
    (
        int enterpriseId,
        ApplicationDbContext dbContext,
        ClaimsPrincipal claimsPrincipal
    )
    {
        if (!claimsPrincipal.IsInRole(RoleNames.Manager))
            return true;

        var userId = ApplicationUser.GetUserId(claimsPrincipal);

        var enterpriseManager = await dbContext.EnterprisesManagers
            .FirstOrDefaultAsync(em => em.EnterpriseId == enterpriseId && em.ManagerId == userId);

        return enterpriseManager is not null;
    }
    public DateTimeOffset ConvertTimeToTimeZone(DateTime dateTime)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

        return new DateTimeOffset(dateTime, TimeSpan.Zero)
            .ToOffset(timeZone.GetUtcOffset(dateTime));
    }
    public static IQueryable<Enterprise> GetUserEnterprises
    (
        ApplicationDbContext dbContext,
        ClaimsPrincipal claimsPrincipal
    )
    {
        if (claimsPrincipal.IsInRole(RoleNames.Admin))
            return dbContext.Enterprises;

        if (claimsPrincipal.IsInRole(RoleNames.Manager))
        {
            var userId = ApplicationUser.GetUserId(claimsPrincipal);

            return dbContext.Enterprises
                .Include(e => e.EnterprisesManagers)
                .Where(e => e.EnterprisesManagers.Any(em => em.ManagerId == userId));
        }

        throw new Exception($"User role should be {RoleNames.Admin} or {RoleNames.Manager}");
    }
    public async Task SaveAndBindWithManager
    (
        ApplicationDbContext dbContext,
        ClaimsPrincipal claimsPrincipal
    )
    {
        dbContext.Add(this);
        await dbContext.SaveChangesAsync();

        if (claimsPrincipal.IsInRole(RoleNames.Manager))
        {
            var savedEnterprise = await dbContext.Enterprises
                .Where(e => e.Name == this.Name && e.City == this.City)
                .FirstOrDefaultAsync();

            dbContext.Add(new EnterpriseManager
            {
                EnterpriseId = savedEnterprise.Id,
                ManagerId = ApplicationUser.GetUserId(claimsPrincipal),
            });

            await dbContext.SaveChangesAsync();
        }
    }
}
