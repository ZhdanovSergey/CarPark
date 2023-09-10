using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using CarPark.Migrations;

namespace CarPark.Controllers
{
    public class DriversController : Controller
    {
        private readonly AppDbContext _context;

        public DriversController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Drivers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Drivers
                .Include(d => d.Enterprise);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(d => d.Enterprise)
                .Include(d => d.DriversVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // GET: Drivers/Create
        public IActionResult Create()
        {
            ViewData["Enterprises"] = new SelectList(_context.Enterprises, "Id", "Name");
            return View();
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EnterpriseId,Name,Salary")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Enterprises"] = new SelectList(_context.Enterprises, "Id", "Name", driver.EnterpriseId);

            return View(driver);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(d => d.DriversVehicles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null)
            {
                return NotFound();
            }

            ViewData["Enterprises"] = new SelectList(_context.Enterprises, "Id", "Name", driver.EnterpriseId);
            ViewData["Vehicles"] = new MultiSelectList(_context.Vehicles.Where(v => v.EnterpriseId == driver.EnterpriseId), "Id", "RegistrationNumber");
            driver.SelectedVehiclesIds = driver.DriversVehicles.Select(m => m.VehicleId).ToList();

            return View(driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Salary,EnterpriseId,SelectedVehiclesIds")]  Driver driver)
        {
            if (id != driver.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var driversVehicles = await _context.DriversVehicles
                        .Where(dv => dv.DriverId == driver.Id)
                        .ToListAsync();

                    //var validSelectedVehiclesIds = _context.Vehicles
                    //    .Where(sv => sv.EnterpriseId == driver.EnterpriseId)
                    //    .Where(v => driver.SelectedVehiclesIds.Any(svid => svid == v.Id))
                    //    .Select(sv => sv.Id)
                    //    .ToListAsync();

                    foreach (var driverVehicle in driversVehicles)
                    {
                        if (!driver.SelectedVehiclesIds.Any(id => id == driverVehicle.VehicleId))
                        {
                            _context.DriversVehicles.Remove(driverVehicle);
                        }
                    }

                    foreach (var selectedVehicleId in driver.SelectedVehiclesIds)
                    {
                        if (!driversVehicles.Any(m => m.VehicleId == selectedVehicleId))
                        {
                            _context.DriversVehicles.Add(new Models.DriverVehicle { DriverId = driver.Id, VehicleId = selectedVehicleId });
                        }
                    }

                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.Id))
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

            ViewData["Enterprises"] = new SelectList(_context.Enterprises, "Id", "Name", driver.EnterpriseId);
            ViewData["Vehicles"] = new MultiSelectList(_context.Vehicles, "Id", "RegistrationNumber");

            return View(driver);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(d => d.Enterprise)
                .Include(d => d.DriversVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Drivers == null)
            {
                return Problem("Entity set 'AppDbContext.Driver'  is null.");
            }
            var driver = await _context.Drivers.FindAsync(id);
            if (driver != null)
            {
                _context.Drivers.Remove(driver);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
          return (_context.Drivers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
