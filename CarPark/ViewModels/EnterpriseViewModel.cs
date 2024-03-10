using CarPark.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarPark.ViewModels;

public class EnterpriseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public TimeSpan UtcOffset { get; set; }
    public string? TimeZoneId { get; set; }
    public SelectList? TimeZoneSelectList { get; set; }
    public EnterpriseViewModel()
    {
        TimeZoneSelectList = new SelectList(TimeZoneInfo.GetSystemTimeZones(), "Id", "Id", this.TimeZoneId);
    }
    public EnterpriseViewModel(Enterprise enterprise) : this()
    {
        Id = enterprise.Id;
        Name = enterprise.Name;
        City = enterprise.City;
        TimeZoneId = enterprise.TimeZoneId;
    }
}
