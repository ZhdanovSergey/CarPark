namespace CarPark.Models;

public class Location
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public DateTime Time { get; set; }

    double latitude;
    public double Latitude 
    {
        get => latitude;
        set
        {
            if (value >= -90 && value <= 90 && !double.IsNaN(value) && !double.IsInfinity(value))
                latitude = value;
            else
                throw new InvalidOperationException($"Failed attempt to set invalid latitude value: {value}");
        }
    }
    double longitude;
    public double Longitude
    {
        get => longitude;
        set
        {
            if (value >= -180 && value <= 180 && !double.IsNaN(value) && !double.IsInfinity(value))
                longitude = value;
            else
                throw new InvalidOperationException($"Failed attempt to set invalid longitude value: {value}");
        }
    }
}
