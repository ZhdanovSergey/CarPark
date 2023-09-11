﻿// <auto-generated />
using System;
using CarPark.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarPark.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CarPark.Models.Brand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LoadCapacity")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfSeats")
                        .HasColumnType("int");

                    b.Property<int>("TankCapacity")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Brands", (string)null);
                });

            modelBuilder.Entity("CarPark.Models.Driver", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EnterpriseId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Salary")
                        .HasColumnType("int");

                    b.Property<int?>("VehicleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EnterpriseId");

                    b.HasIndex("VehicleId")
                        .IsUnique()
                        .HasFilter("[VehicleId] IS NOT NULL");

                    b.ToTable("Drivers", (string)null);
                });

            modelBuilder.Entity("CarPark.Models.DriverVehicle", b =>
                {
                    b.Property<int>("EnterpriseId")
                        .HasColumnType("int");

                    b.Property<int>("DriverId")
                        .HasColumnType("int");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.HasKey("EnterpriseId", "DriverId", "VehicleId");

                    b.HasIndex("DriverId");

                    b.HasIndex("VehicleId");

                    b.ToTable("DriversVehicles", (string)null);
                });

            modelBuilder.Entity("CarPark.Models.Enterprise", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Enterprises", (string)null);
                });

            modelBuilder.Entity("CarPark.Models.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<int?>("DriverId")
                        .HasColumnType("int");

                    b.Property<int>("EnterpriseId")
                        .HasColumnType("int");

                    b.Property<int>("Mileage")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("RegistrationNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("EnterpriseId");

                    b.ToTable("Vehicles", (string)null);
                });

            modelBuilder.Entity("CarPark.Models.Driver", b =>
                {
                    b.HasOne("CarPark.Models.Enterprise", "Enterprise")
                        .WithMany("Drivers")
                        .HasForeignKey("EnterpriseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CarPark.Models.Vehicle", "Vehicle")
                        .WithOne("Driver")
                        .HasForeignKey("CarPark.Models.Driver", "VehicleId");

                    b.Navigation("Enterprise");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("CarPark.Models.DriverVehicle", b =>
                {
                    b.HasOne("CarPark.Models.Driver", "Driver")
                        .WithMany("DriversVehicles")
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarPark.Models.Enterprise", "Enterprise")
                        .WithMany("DriversVehicles")
                        .HasForeignKey("EnterpriseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarPark.Models.Vehicle", "Vehicle")
                        .WithMany("DriversVehicles")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Driver");

                    b.Navigation("Enterprise");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("CarPark.Models.Vehicle", b =>
                {
                    b.HasOne("CarPark.Models.Brand", "Brand")
                        .WithMany("Vehicles")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarPark.Models.Enterprise", "Enterprise")
                        .WithMany("Vehicles")
                        .HasForeignKey("EnterpriseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Enterprise");
                });

            modelBuilder.Entity("CarPark.Models.Brand", b =>
                {
                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("CarPark.Models.Driver", b =>
                {
                    b.Navigation("DriversVehicles");
                });

            modelBuilder.Entity("CarPark.Models.Enterprise", b =>
                {
                    b.Navigation("Drivers");

                    b.Navigation("DriversVehicles");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("CarPark.Models.Vehicle", b =>
                {
                    b.Navigation("Driver");

                    b.Navigation("DriversVehicles");
                });
#pragma warning restore 612, 618
        }
    }
}
