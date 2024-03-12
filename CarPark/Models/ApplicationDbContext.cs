using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CarPark.Models;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    const int SPATIAL_REF_ID = 4326;
    public DbSet<Brand> Brands { get; init; } = null!;
    public DbSet<Driver> Drivers { get; init; } = null!;
    public DbSet<DriverVehicle> DriversVehicles { get; init; } = null!;
    public DbSet<Enterprise> Enterprises { get; init; } = null!;
    public DbSet<EnterpriseManager> EnterprisesManagers { get; init; } = null!;
    public DbSet<Location> Locations { get; init; } = null!;
    public DbSet<Manager> Managers { get; init; } = null!;
    public DbSet<Ride> Rides { get; init; } = null!;
    public DbSet<Vehicle> Vehicles { get; init; } = null!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Enterprise>()
            .HasMany(e => e.Drivers)
            .WithOne(d => d.Enterprise)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enterprise>()
            .HasMany(e => e.Vehicles)
            .WithOne(v => v.Enterprise)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EnterpriseManager>()
            .HasKey(em => new { em.EnterpriseId, em.ManagerId });

        modelBuilder.Entity<DriverVehicle>()
            .HasKey(dv => new { dv.EnterpriseId, dv.DriverId, dv.VehicleId });

        modelBuilder.Entity<Location>()
            .Property(l => l.Point)
            .HasConversion(
                p => p == null ? null : new Point(p.X, p.Y) { SRID = SPATIAL_REF_ID },
                p => p == null ? null : new Point(p.X, p.Y) { SRID = SPATIAL_REF_ID }
            );
    }
}
