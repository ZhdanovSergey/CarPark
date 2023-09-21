using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
                return NotFound();

            var driver = await _context.Drivers
                .Include(d => d.ActiveVehicle)
                .Include(d => d.DriversVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                .Include(d => d.Enterprise)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                return NotFound();

            return View(driver);
        }

        // GET: Drivers/Create
        public async Task<IActionResult> Create()
        {
            var driverVM = new DriverViewModel();
            await driverVM.AddCreateSelectLists(_context);

            return View(driverVM);
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Salary,EnterpriseId")] DriverViewModel driverVM)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new Driver(driverVM));
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await driverVM.AddCreateSelectLists(_context);

            return View(driverVM);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Drivers == null)
                return NotFound();

            var driver = await _context.Drivers
                .Include(d => d.DriversVehicles)
                .Include(d => d.ActiveVehicle)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null)
                return NotFound();

            var vehiclesIds = await _context.DriversVehicles
                .Where(dv => dv.DriverId == driver.Id)
                .Select(dv => dv.VehicleId)
                .ToListAsync();

            var activeVehicleId = (await _context.Drivers
                .Include(d => d.ActiveVehicle)
                .FirstOrDefaultAsync(d => d.Id == driver.Id))?.ActiveVehicle?.Id;

            var driverVM = new DriverViewModel(driver);
            await driverVM.AddEditSelectLists(_context);

            return View(driverVM);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Salary,EnterpriseId,VehiclesIds,ActiveVehicleId")] DriverViewModel driverVM)
        {
            if (id != driverVM.Id)
                return NotFound();

            var driversVehicles = await _context.DriversVehicles
                .Where(dv => dv.DriverId == driverVM.Id)
                .ToListAsync();

            if (ModelState.IsValid)
            {
                try
                {
                    var validSelectedVehiclesIds = await _context.Vehicles
                        .Where(v => driverVM.VehiclesIds.Any(svid => svid == v.Id)
                            && v.EnterpriseId == driverVM.EnterpriseId)
                        .Select(v => v.Id)
                        .ToListAsync();

                    foreach (var driverVehicle in driversVehicles)
                    {
                        if (driverVehicle.EnterpriseId != driverVM.EnterpriseId
                            || !validSelectedVehiclesIds.Any(id => id == driverVehicle.VehicleId))
                        {
                            _context.DriversVehicles.Remove(driverVehicle);
                        }
                    }

                    foreach (var validSelectedVehicleId in validSelectedVehiclesIds)
                    {
                        if (!driversVehicles.Any(m => m.VehicleId == validSelectedVehicleId))
                        {
                            _context.DriversVehicles.Add(new Models.DriverVehicle
                            {
                                EnterpriseId = driverVM.EnterpriseId,
                                DriverId = driverVM.Id,
                                VehicleId = validSelectedVehicleId
                            });
                        }
                    }

                    if (validSelectedVehiclesIds.Any(svid => svid == driverVM.ActiveVehicleId))
                    {
                        var activeVehicle = await _context.Vehicles
                            .FirstOrDefaultAsync(v => v.Id == driverVM.ActiveVehicleId);

                        if (activeVehicle is not null)
                        {
                            activeVehicle.ActiveDriverId = driverVM.Id;
                            _context.Update(activeVehicle);
                        }
                    } else
                    {
                        var prevActiveVehicle = await _context.Vehicles
                            .FirstOrDefaultAsync(v => v.ActiveDriverId == driverVM.Id);

                        if (prevActiveVehicle != null)
                        {
                            prevActiveVehicle.ActiveDriverId = null;
                            _context.Update(prevActiveVehicle);
                        }

                    }

                    _context.Update(new Driver(driverVM));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driverVM.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await driverVM.AddEditSelectLists(_context);

            return View(driverVM);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Drivers == null)
                return NotFound();

            var driver = await _context.Drivers
                .Include(v => v.ActiveVehicle)
                .Include(d => d.Enterprise)
                .Include(d => d.DriversVehicles)
                    .ThenInclude(dv => dv.Vehicle)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                return NotFound();

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Drivers == null)
                return Problem("Entity set 'AppDbContext.Driver'  is null.");

            var driver = await _context.Drivers.FindAsync(id);

            if (driver != null)
                _context.Drivers.Remove(driver);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
          return (_context.Drivers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
