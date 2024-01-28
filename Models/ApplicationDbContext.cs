using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Xml;

namespace CarPark.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        const int SPATIAL_REF_ID = 4326;
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<DriverVehicle> DriversVehicles { get; set; }
        public DbSet<EnterpriseManager> EnterprisesManagers { get; set; }
        public DbSet<Brand> Brands { get; set; }
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
}
