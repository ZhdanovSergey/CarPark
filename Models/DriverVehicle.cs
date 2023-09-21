using CarPark.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarPark.Models
{
    public class DriverVehicle
    {
        public int EnterpriseId { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public Driver? Driver { get; set; }
        public Vehicle? Vehicle { get; set; }
        public static void Update(AppDbContext dbContext, IEnumerable<DriverVehicle> oldData, IEnumerable<DriverVehicle> newData, DriverVehicleIdProp comparedIdProp)
        {
            foreach (var oldDataItem in oldData)
            {
                if (!newData.Any(newDataItem => newDataItem[comparedIdProp] == oldDataItem[comparedIdProp]))
                    dbContext.DriversVehicles.Remove(oldDataItem);
            }

            foreach (var newDataItem in newData)
            {
                if (!oldData.Any(oldDataItem => oldDataItem[comparedIdProp] == newDataItem[comparedIdProp]))
                    dbContext.DriversVehicles.Add(newDataItem);
            }
        }
        int this[DriverVehicleIdProp idProp]
        {
            get => idProp switch
            {
                DriverVehicleIdProp.DriverId => this.DriverId,
                DriverVehicleIdProp.VehicleId => this.VehicleId,
            };
        }
    }
    public enum DriverVehicleIdProp
    {
        DriverId,
        VehicleId
    }
}
