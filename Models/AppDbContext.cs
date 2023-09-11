using Microsoft.EntityFrameworkCore;

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
                .HasKey(m => new { m.EnterpriseId, m.DriverId, m.VehicleId });

            modelBuilder.Entity<Enterprise>()
                .HasMany(m => m.Drivers)
                .WithOne(m => m.Enterprise)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enterprise>()
                .HasMany(m => m.Vehicles)
                .WithOne(m => m.Enterprise)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
