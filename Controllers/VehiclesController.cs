using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;

namespace CarPark.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly AppDbContext _context;

        public VehiclesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Lists
        public async Task<IActionResult> Index()
        {
              return _context.Vehicles != null ? 
                          View(await _context.Vehicles.Include(m => m.Brand).Include(m => m.Enterprise).ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Vehicles'  is null.");
        }

        // GET: Lists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Enterprise)
                .Include(v => v.DriversVehicles)
                    .ThenInclude(d => d.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Lists/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Brands"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["Enterprises"] = new SelectList(_context.Enterprises, "Id", "Name");
            return View();
        }

        // POST: Lists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EnterpriseId,BrandId,Price,RegistrationNumber,Year,Mileage")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Brands"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
            ViewData["Enterprises"] = new SelectList(_context.Enterprises, "Id", "Name", vehicle.EnterpriseId);
            return View(vehicle);
        }

        // GET: Lists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(d => d.DriversVehicles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            ViewData["Brands"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
            ViewData["Enterprises"] = new SelectList(_context.Enterprises, "Id", "Name", vehicle.EnterpriseId);
            ViewData["Drivers"] = new MultiSelectList(_context.Drivers, "Id", "Name");
            vehicle.SelectedDriversIds = vehicle.DriversVehicles.Select(m => m.DriverId).ToList();
            return View(vehicle);
        }

        // POST: Lists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EnterpriseId,BrandId,Price,RegistrationNumber,Year,Mileage,SelectedDriversIds")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var driversVehicles = await _context.DriversVehicles
                        .Where(dv => dv.VehicleId == vehicle.Id)
                        .ToListAsync();

                    foreach (var driverVehicle in driversVehicles)
                    {
                        if (!vehicle.SelectedDriversIds.Any(id => id == driverVehicle.DriverId))
                        {
                            _context.DriversVehicles.Remove(driverVehicle);
                        }
                    }

                    foreach (var selectedDriverId in vehicle.SelectedDriversIds)
                    {
                        if (!driversVehicles.Any(m => m.DriverId == selectedDriverId))
                        {
                            _context.DriversVehicles.Add(new DriverVehicle { DriverId = selectedDriverId, VehicleId = vehicle.Id });
                        }
                    }

                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Brands"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
            ViewData["Enterprises"] = new SelectList(_context.Enterprises, "Id", "Name", vehicle.EnterpriseId);
            ViewData["Drivers"] = new MultiSelectList(_context.Drivers, "Id", "Name");
            return View(vehicle);
        }

        // GET: Lists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Enterprise)
                .Include(v => v.DriversVehicles)
                    .ThenInclude(dv => dv.Driver)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Lists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'AppDbContext.Vehicles'  is null.");
            }
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
          return (_context.Vehicles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
