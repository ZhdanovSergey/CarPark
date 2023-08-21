﻿namespace CarPark.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public int LoadCapacity { get; set; }
        public string Name { get; set; }
        public int NumberOfSeats { get; set; }
        public int TankCapacity { get; set; }
        public string Type { get; set; }
        public List<Vehicle> Vehicles { get; set; } = new();
    }
}