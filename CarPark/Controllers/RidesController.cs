using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CarPark.APIModels;

namespace CarPark.Controllers;

[Authorize]
public sealed class RidesController : Controller
{
    private readonly ApplicationDbContext _context;

    public RidesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Rides
    public async Task<IActionResult> Index(int vehicleId, int[] ids)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle is null)
            return NotFound();

        if (!(await Enterprise.CheckAccess(vehicle.EnterpriseId, _context, User)))
            return Forbid();

        var groupedPoints = new List<List<PointDTO>>();

        var rides = await _context.Rides
            .Where(r => r.VehicleId == vehicleId)
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();

        if (rides.Count() == 0)
            return NotFound();

        foreach (var ride in rides)
        {
            var points = await _context.Locations
                .Where(location => location.VehicleId == ride.VehicleId)
                .Where(location => location.DateTime >= ride.Start)
                .Where(location => location.DateTime <= ride.End)
                .Select(location => new PointDTO { X = location.Point.X, Y = location.Point.Y })
                .ToListAsync();

            groupedPoints.Add(points);
        }

        return View(groupedPoints);
    }

    //// GET: Rides/Details/5
    //public async Task<IActionResult> Details(int? id)
    //{
    //    if (id == null || _context.Rides == null)
    //    {
    //        return NotFound();
    //    }

    //    var ride = await _context.Rides
    //        .Include(r => r.Vehicle)
    //        .FirstOrDefaultAsync(m => m.Id == id);
    //    if (ride == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(ride);
    //}

    //// GET: Rides/Create
    //public IActionResult Create()
    //{
    //    ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id");
    //    return View();
    //}

    //// POST: Rides/Create
    //// To protect from overposting attacks, enable the specific properties you want to bind to.
    //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create([Bind("Id,VehicleId,Start,End")] Ride ride)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        _context.Add(ride);
    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }
    //    ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", ride.VehicleId);
    //    return View(ride);
    //}

    //// GET: Rides/Edit/5
    //public async Task<IActionResult> Edit(int? id)
    //{
    //    if (id == null || _context.Rides == null)
    //    {
    //        return NotFound();
    //    }

    //    var ride = await _context.Rides.FindAsync(id);
    //    if (ride == null)
    //    {
    //        return NotFound();
    //    }
    //    ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", ride.VehicleId);
    //    return View(ride);
    //}

    //// POST: Rides/Edit/5
    //// To protect from overposting attacks, enable the specific properties you want to bind to.
    //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleId,Start,End")] Ride ride)
    //{
    //    if (id != ride.Id)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            _context.Update(ride);
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!RideExists(ride.Id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //        return RedirectToAction(nameof(Index));
    //    }
    //    ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", ride.VehicleId);
    //    return View(ride);
    //}

    //// GET: Rides/Delete/5
    //public async Task<IActionResult> Delete(int? id)
    //{
    //    if (id == null || _context.Rides == null)
    //    {
    //        return NotFound();
    //    }

    //    var ride = await _context.Rides
    //        .Include(r => r.Vehicle)
    //        .FirstOrDefaultAsync(m => m.Id == id);
    //    if (ride == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(ride);
    //}

    //// POST: Rides/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(int id)
    //{
    //    if (_context.Rides == null)
    //    {
    //        return Problem("Entity set 'ApplicationDbContext.Rides'  is null.");
    //    }
    //    var ride = await _context.Rides.FindAsync(id);
    //    if (ride != null)
    //    {
    //        _context.Rides.Remove(ride);
    //    }

    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}

    //private bool RideExists(int id)
    //{
    //  return (_context.Rides?.Any(e => e.Id == id)).GetValueOrDefault();
    //}
}
