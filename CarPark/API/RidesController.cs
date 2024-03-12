using System;
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

namespace CarPark.API;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class RidesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RidesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Rides
    [HttpGet]
    public async Task<ActionResult<string>> GetRides(int vehicleId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, bool geoJson = false)
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
                Point = new LocationDTOPoint { X = l.Point.X, Y = l.Point.Y }
            });

        return JsonSerializer.Serialize(locationsWithSimplePoints);
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
