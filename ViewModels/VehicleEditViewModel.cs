using CarPark.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPark.ViewModels
{
    public class VehicleEditViewModel
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
    }
}
