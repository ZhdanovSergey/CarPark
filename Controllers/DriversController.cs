using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using CarPark.Migrations;
using CarPark.ViewModels;

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
            var drivers = await _context.Drivers
                .Include(d => d.ActiveVehicle)
                .Include(d => d.Enterprise)
                .ToListAsync();

            return View(drivers);
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(d => d.ActiveVehicle)
                .Include(d => d.DriversVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                .Include(d => d.Enterprise)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // GET: Drivers/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Enterprises"] = new SelectList(await _context.Enterprises.ToListAsync(), "Id", "Name");
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

            ViewData["Enterprises"] = new SelectList(await _context.Enterprises.ToListAsync(), "Id", "Name", driver.EnterpriseId);

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
                .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null)
            {
                return NotFound();
            }

            return View(await DriverEditViewModel.CreateNewAsync(driver, _context));
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Salary,EnterpriseId,VehiclesIds,ActiveVehicleId")] DriverEditViewModel driverEdit)
        {
            if (id != driverEdit.Id)
            {
                return NotFound();
            }

            var driversVehicles = await _context.DriversVehicles
                .Where(dv => dv.DriverId == driverEdit.Id)
                .ToListAsync();

            if (ModelState.IsValid)
            {
                try
                {
                    var validSelectedVehiclesIds = await _context.Vehicles
                        .Where(v => driverEdit.VehiclesIds.Any(svid => svid == v.Id)
                            && v.EnterpriseId == driverEdit.EnterpriseId)
                        .Select(v => v.Id)
                        .ToListAsync();

                    foreach (var driverVehicle in driversVehicles)
                    {
                        if (driverVehicle.EnterpriseId != driverEdit.EnterpriseId
                            || !validSelectedVehiclesIds.Any(id => id == driverVehicle.VehicleId))
                        {
                            _context.DriversVehicles.Remove(driverVehicle);
                        }
                    }

                    foreach (var validSelectedVehicleId in validSelectedVehiclesIds)
                    {
                        if (!driversVehicles.Any(m => m.VehicleId == validSelectedVehicleId))
                        {
                            _context.DriversVehicles
                                .Add(new Models.DriverVehicle
                                {
                                    EnterpriseId = driverEdit.EnterpriseId,
                                    DriverId = driverEdit.Id,
                                    VehicleId = validSelectedVehicleId
                                });
                        }
                    }

                    if (validSelectedVehiclesIds.Any(svid => svid == driverEdit.ActiveVehicleId))
                    {
                        var activeVehicle = await _context.Vehicles
                            .FirstOrDefaultAsync(v => v.Id == driverEdit.ActiveVehicleId);

                        if (activeVehicle is not null)
                        {
                            activeVehicle.ActiveDriverId = driverEdit.Id;
                            _context.Update(activeVehicle);
                        }
                    } else
                    {
                        var prevActiveVehicle = await _context.Vehicles
                            .FirstOrDefaultAsync(v => v.ActiveDriverId == driverEdit.Id);

                        if (prevActiveVehicle != null)
                        {
                            prevActiveVehicle.ActiveDriverId = null;
                            _context.Update(prevActiveVehicle);
                        }

                    }

                    _context.Update((Driver)driverEdit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driverEdit.Id))
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

            return View(await DriverEditViewModel.CreateNewAsync(driverEdit, _context));
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(v => v.ActiveVehicle)
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
