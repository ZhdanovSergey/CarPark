﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using CarPark.ViewModels;

namespace CarPark.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lists
        public async Task<IActionResult> Index()
        {
            return _context.Vehicles != null ? 
                View(await _context.Vehicles
                    .Include(m => m.ActiveDriver)
                    .Include(m => m.Brand)
                    .Include(m => m.Enterprise)
                    .ToListAsync()) :
                Problem("Entity set 'AppDbContext.Vehicles'  is null.");
        }

        // GET: Lists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vehicles == null)
                return NotFound();

            var vehicle = await _context.Vehicles
                .Include(m => m.ActiveDriver)
                .Include(v => v.Brand)
                .Include(v => v.Enterprise)
                .Include(v => v.DriversVehicles)
                    .ThenInclude(d => d.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        // GET: Lists/Create
        public async Task<IActionResult> Create()
        {
            var vehicleVM = new VehicleViewModel();
            await vehicleVM.AddCreateSelectLists(_context);

            return View(vehicleVM);
        }

        // POST: Lists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EnterpriseId,BrandId,Price,RegistrationNumber,Year,Mileage")] VehicleViewModel vehicleVM)
        {
            if (ModelState.IsValid)
            {
                var vehicle = new Vehicle(vehicleVM);
                _context.Add(vehicle);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await vehicleVM.AddCreateSelectLists(_context);

            return View(vehicleVM);
        }

        // GET: Lists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vehicles == null)
                return NotFound();

            var vehicle = await _context.Vehicles
                .Include(d => d.DriversVehicles)
                    .ThenInclude(dv => dv.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
                return NotFound();

            var vehicleVM = new VehicleViewModel(vehicle);
            await vehicleVM.AddEditSelectLists(_context);

            return View(vehicleVM);
        }

        // POST: Lists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EnterpriseId,BrandId,Price,RegistrationNumber,Year,Mileage,DriversIds,ActiveDriverId")] VehicleViewModel vehicleVM)
        {
            if (id != vehicleVM.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var oldDriversVehicles = await _context.DriversVehicles
                        .Where(dv => dv.VehicleId == vehicleVM.Id)
                        .ToListAsync();

                    var newDriversVehicles = await _context.Drivers
                        .Where(driver => driver.EnterpriseId == vehicleVM.EnterpriseId
                            && vehicleVM.DriversIds.Any(did => did == driver.Id))
                        .Select(driver => new DriverVehicle
                        {
                            EnterpriseId = vehicleVM.EnterpriseId,
                            DriverId = driver.Id,
                            VehicleId = vehicleVM.Id,
                        }).ToListAsync();

                    DriverVehicle.Update(_context, oldDriversVehicles, newDriversVehicles, DriverVehicleIdProp.DriverId);

                    vehicleVM.ActiveDriverId = newDriversVehicles
                        .Any(dv => dv.DriverId == vehicleVM.ActiveDriverId)
                        ? vehicleVM.ActiveDriverId
                        : null;

                    _context.Update(new Vehicle(vehicleVM));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicleVM.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await vehicleVM.AddEditSelectLists(_context);

            return View(vehicleVM);
        }

        // GET: Lists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicles == null)
                return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.ActiveDriver)
                .Include(v => v.Brand)
                .Include(v => v.Enterprise)
                .Include(v => v.DriversVehicles)
                    .ThenInclude(dv => dv.Driver)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        // POST: Lists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vehicles == null)
                return Problem("Entity set 'AppDbContext.Vehicles'  is null.");

            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle != null)
                _context.Vehicles.Remove(vehicle);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
          return (_context.Vehicles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
