using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
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

            modelBuilder.Entity<DriverVehicle>()
                .HasKey(dv => new { dv.EnterpriseId, dv.DriverId, dv.VehicleId });

            modelBuilder.Entity<EnterpriseManager>()
                .HasKey(em => new { em.EnterpriseId, em.ManagerId });
        }
    }
}
