﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using Microsoft.AspNetCore.Authorization;
using CarPark.APIModels;
using NetTopologySuite.IO;
using System.Text.Json;
using NetTopologySuite.Geometries;
using System.Text;
using System.Collections;
using System.Net;
using System.Web;
using System.Globalization;

namespace CarPark.API;

[Authorize]
[ApiController]
public sealed class RidesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RidesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/rides-as-locations/{vehicleId}")]
    public async Task<ActionResult<string>> GetRidesAsLocations(int vehicleId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, bool geoJson = false)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle is null)
            return NotFound();

        if (!(await Enterprise.CheckAccess(vehicle.EnterpriseId, _context, User)))
            return StatusCode(StatusCodes.Status403Forbidden);

        var locationsWithTimezone = _context.Rides
            .Where(ride => ride.VehicleId == vehicleId)
            .Where(ride => dateFrom == null || ride.Start >= dateFrom)
            .Where(ride => dateTo == null || ride.End <= dateTo)
            .SelectMany(ride => _context.Locations
                .Where(location => location.VehicleId == ride.VehicleId)
                .Where(location => location.DateTime >= ride.Start)
                .Where(location => location.DateTime <= ride.End))
            .Select(l => new
            {
                l.Id,
                l.VehicleId,
                l.Point,
                DateTime = vehicle.Enterprise.ConvertTimeToTimeZone(l.DateTime)
            });

        if (geoJson)
        {
            var geoJsonWriter = new GeoJsonWriter();
            return geoJsonWriter.Write(locationsWithTimezone);
        }

        var locationsWithSimplePoints = locationsWithTimezone
            .Select(l => new LocationDTO
            {
                Id = l.Id,
                VehicleId = l.VehicleId,
                DateTime = l.DateTime,
                Point = new PointDTO { X = l.Point.X, Y = l.Point.Y }
            });

        return JsonSerializer.Serialize(locationsWithSimplePoints);
    }

    [HttpGet]
    [Route("api/rides/{vehicleId}")]
    public async Task<ActionResult<IEnumerable<RideInfo>>> GetRides(int vehicleId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle is null)
            return NotFound();

        if (!(await Enterprise.CheckAccess(vehicle.EnterpriseId, _context, User)))
            return StatusCode(StatusCodes.Status403Forbidden);

        var rides = await _context.Rides
            .Where(ride => ride.VehicleId == vehicleId)
            .Where(ride => dateFrom == null || ride.Start >= dateFrom)
            .Where(ride => dateTo == null || ride.End <= dateTo)
            .ToListAsync();

        var locations = await _context.Locations
            .Where(location => location.VehicleId == vehicleId)
            .Where(location => dateFrom == null || location.DateTime >= dateFrom)
            .Where(location => dateTo == null || location.DateTime <= dateTo)
            .ToListAsync();

        var rideInfoTasks = rides.Select(ride => CreateRideInfo(vehicle, ride, locations));

        return await Task.WhenAll(rideInfoTasks);
    }

    private static async Task<RideInfo> CreateRideInfo(Vehicle vehicle, Ride ride, IEnumerable<Models.Location> allLocations)
    {
        var rideLocations = allLocations
            .Where(location => location.DateTime >= ride.Start)
            .Where(location => location.DateTime <= ride.End);

        var startLocation = rideLocations.MinBy(l => l.DateTime);
        var endLocation = rideLocations.MaxBy(l => l.DateTime);

        var startEdge = startLocation == null ? null : new RideInfoEdge
        {
            DateTime = vehicle.Enterprise.ConvertTimeToTimeZone(startLocation.DateTime),
            Point = new PointDTO { X = startLocation.Point.X, Y = startLocation.Point.Y },
            PhysicalAdress = await GetPhysicalAdress(startLocation.Point),
        };

        var endEdge = endLocation == null ? null : new RideInfoEdge
        {
            DateTime = vehicle.Enterprise.ConvertTimeToTimeZone(endLocation.DateTime),
            Point = new PointDTO { X = endLocation.Point.X, Y = endLocation.Point.Y },
            PhysicalAdress = await GetPhysicalAdress(endLocation.Point),
        };

        return new RideInfo
        {
            Id = ride.Id,
            VehicleId = vehicle.Id,
            Start = startEdge,
            End = endEdge,
        };
    }

    private static async Task<string> GetPhysicalAdress(Point point)
    {
        const string API_URL = "https://api.openrouteservice.org/geocode/reverse";
        const string API_KEY = "5b3ce3597851110001cf624805ffbce60b8e4f0aa6fca9a0766f931b";

        var queryString = HttpUtility.ParseQueryString("");
        queryString.Add("api_key", API_KEY);
        queryString.Add("point.lon", point.X.ToString("0.#####", CultureInfo.InvariantCulture));
        queryString.Add("point.lat", point.Y.ToString("0.#####", CultureInfo.InvariantCulture));
        queryString.Add("size", 1.ToString());
        queryString.Add("layers", "address");

        var builder = new UriBuilder(API_URL);
        builder.Query = queryString.ToString();
        string uri = builder.Uri.ToString();

        using var apiClient = new HttpClient();
        var response = await apiClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(response.ReasonPhrase);

        var deserializedResponse = await response.Content.ReadFromJsonAsync<OpenRouteResponse>();
        return deserializedResponse?.Features[0].Properties.Label ?? "";
    }

    //// GET: api/Rides/5
    //[HttpGet("{id}")]
    //public async Task<ActionResult<Ride>> GetRide(int id)
    //{
    //  if (_context.Rides == null)
    //  {
    //      return NotFound();
    //  }
    //    var ride = await _context.Rides.FindAsync(id);

    //    if (ride == null)
    //    {
    //        return NotFound();
    //    }

    //    return ride;
    //}

    //// PUT: api/Rides/5
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutRide(int id, Ride ride)
    //{
    //    if (id != ride.Id)
    //    {
    //        return BadRequest();
    //    }

    //    _context.Entry(ride).State = EntityState.Modified;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!RideExists(id))
    //        {
    //            return NotFound();
    //        }
    //        else
    //        {
    //            throw;
    //        }
    //    }

    //    return NoContent();
    //}

    //// POST: api/Rides
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPost]
    //public async Task<ActionResult<Ride>> PostRide(Ride ride)
    //{
    //  if (_context.Rides == null)
    //  {
    //      return Problem("Entity set 'ApplicationDbContext.Rides'  is null.");
    //  }
    //    _context.Rides.Add(ride);
    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction("GetRide", new { id = ride.Id }, ride);
    //}

    //// DELETE: api/Rides/5
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteRide(int id)
    //{
    //    if (_context.Rides == null)
    //    {
    //        return NotFound();
    //    }
    //    var ride = await _context.Rides.FindAsync(id);
    //    if (ride == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.Rides.Remove(ride);
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}

    //private bool RideExists(int id)
    //{
    //    return (_context.Rides?.Any(e => e.Id == id)).GetValueOrDefault();
    //}
}
