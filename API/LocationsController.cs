﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using NetTopologySuite.IO;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace CarPark.API;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LocationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Locations/5
    [HttpGet("{vehicleId}")]
    public async Task<ActionResult<string>> GetLocations(int vehicleId, DateTime? dateFrom, DateTime? dateTo, bool geoJson = false)
    {
        var vehicle = _context.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefault(v => v.Id == vehicleId);

        if (vehicle is null || _context.Locations is null)
            return NotFound();

        if (!(await Enterprise.CheckAccess(vehicle.EnterpriseId, _context, User)))
            return StatusCode(StatusCodes.Status403Forbidden);

        var locationsWithTimezone = _context.Locations
            .Where(l => vehicleId == null || l.VehicleId == vehicleId)
            .Where(l => dateFrom == null || l.DateTime >= dateFrom)
            .Where(l => dateTo == null || l.DateTime <= dateTo)
            .Select(l => new
            {
                l.Id, l.VehicleId, l.Point,
                DateTime = vehicle.Enterprise.ConvertTimeToTimeZone(l.DateTime)
            });

        if (geoJson)
        {
            var geoJsonWriter = new GeoJsonWriter();
            return geoJsonWriter.Write(locationsWithTimezone);
        }

        var locationsWithSimplePoints = locationsWithTimezone
            .Select(l => new
            {
                l.Id, l.VehicleId, l.DateTime,
                Point = new { l.Point.X, l.Point.Y }
            });

        return JsonSerializer.Serialize(locationsWithSimplePoints);
    }

    //// GET: api/Locations/5
    //[HttpGet("{id}")]
    //public async Task<ActionResult<Models.Location>> GetLocation(int id)
    //{
    //  if (_context.Locations == null)
    //  {
    //      return NotFound();
    //  }
    //    var location = await _context.Locations.FindAsync(id);

    //    if (location == null)
    //    {
    //        return NotFound();
    //    }

    //    return location;
    //}

    //// PUT: api/Locations/5
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutLocation(int id, Models.Location location)
    //{
    //    if (id != location.Id)
    //    {
    //        return BadRequest();
    //    }

    //    _context.Entry(location).State = EntityState.Modified;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!LocationExists(id))
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

    //// POST: api/Locations
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPost]
    //public async Task<ActionResult<Models.Location>> PostLocation(Models.Location location)
    //{
    //  if (_context.Locations == null)
    //  {
    //      return Problem("Entity set 'ApplicationDbContext.Locations'  is null.");
    //  }
    //    _context.Locations.Add(location);
    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction("GetLocation", new { id = location.Id }, location);
    //}

    //// DELETE: api/Locations/5
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteLocation(int id)
    //{
    //    if (_context.Locations == null)
    //    {
    //        return NotFound();
    //    }
    //    var location = await _context.Locations.FindAsync(id);
    //    if (location == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.Locations.Remove(location);
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}

    //private bool LocationExists(int id)
    //{
    //    return (_context.Locations?.Any(e => e.Id == id)).GetValueOrDefault();
    //}
}
