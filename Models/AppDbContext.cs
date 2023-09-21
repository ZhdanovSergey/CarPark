using CarPark.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace CarPark.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverVehicle> DriversVehicles { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb; Database=CarParkDB; Trusted_Connection=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriverVehicle>()
                .HasKey(dv => new { dv.EnterpriseId, dv.DriverId, dv.VehicleId });

            modelBuilder.Entity<Enterprise>()
                .HasMany(e => e.Drivers)
                .WithOne(d => d.Enterprise)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enterprise>()
                .HasMany(e => e.Vehicles)
                .WithOne(v => v.Enterprise)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
