﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using CarPark.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CarPark.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly UserManager<ApplicationUser> _userManager;

        public VehiclesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lists
        public async Task<IActionResult> Index()
        {
            if (_context.Vehicles == null)
                return Problem("Entity set 'AppDbContext.Vehicles'  is null.");

            var userVehicles = await Vehicle.GetUserVehicles(_context, _userManager, User);

            return View(await userVehicles
                    .Include(m => m.ActiveDriver)
                    .Include(m => m.Brand)
                    .Include(m => m.Enterprise)
                    .ToListAsync());
        }

        // GET: Lists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vehicles == null)
                return NotFound();

            var userVehicles = await Vehicle.GetUserVehicles(_context, _userManager, User);

            var vehicle = await userVehicles
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
            await vehicleVM.AddCreateSelectLists(_context, _userManager, User);
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

            await vehicleVM.AddCreateSelectLists(_context, _userManager, User);
            return View(vehicleVM);
        }

        // GET: Lists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vehicles == null)
                return NotFound();

            var userVehicles = await Vehicle.GetUserVehicles(_context, _userManager, User);

            var vehicle = await userVehicles
                .Include(d => d.DriversVehicles)
                    .ThenInclude(dv => dv.Driver)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
                return NotFound();

            var vehicleVM = new VehicleViewModel(vehicle);
            await vehicleVM.AddEditSelectLists(_context, _userManager, User);

            return View(vehicleVM);
        }

        // POST: Lists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EnterpriseId,BrandId,Price,RegistrationNumber,Year,Mileage,DriversIds,ActiveDriverId")] VehicleViewModel vehicleVM)
        {
            var userVehicles = await Vehicle.GetUserVehicles(_context, _userManager, User);
            var userEnterprises = await Enterprise.GetUserEnterprises(_context, _userManager, User);

            if
            (
                id != vehicleVM.Id
                || await userVehicles.AllAsync(ud => ud.Id != vehicleVM.Id)
                || await userEnterprises.AllAsync(ue => ue.Id != vehicleVM.EnterpriseId)
            )
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldDriversVehicles = await _context.DriversVehicles
                        .Where(dv => dv.VehicleId == vehicleVM.Id)
                        .ToListAsync();

                    var newDriversVehicles = await _context.Drivers
                        .Where(driver => 
                            driver.EnterpriseId == vehicleVM.EnterpriseId
                            && vehicleVM.DriversIds.Any(did => did == driver.Id))
                        .Select(driver => new DriverVehicle
                        {
                            EnterpriseId = vehicleVM.EnterpriseId,
                            DriverId = driver.Id,
                            VehicleId = vehicleVM.Id,
                        }).ToListAsync();

                    DriverVehicle.Update(_context, oldDriversVehicles, newDriversVehicles, DriverVehicleIdProp.DriverId);

                    if (newDriversVehicles.Any(dv => dv.DriverId == vehicleVM.ActiveDriverId))
                        await Vehicle.RemoveActiveDriver(_context, vehicleVM.ActiveDriverId);
                    else
                        vehicleVM.ActiveDriverId = null;

                    _context.Update(new Vehicle(vehicleVM));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await VehicleExists(vehicleVM.Id)))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await vehicleVM.AddEditSelectLists(_context, _userManager, User);
            return View(vehicleVM);
        }

        // GET: Lists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicles == null)
                return NotFound();

            var userVehicles = await Vehicle.GetUserVehicles(_context, _userManager, User);

            var vehicle = await userVehicles
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

            var userVehicles = await Vehicle.GetUserVehicles(_context, _userManager, User);
            var vehicle = await userVehicles.FirstOrDefaultAsync(uv => uv.Id == id);

            if (vehicle != null)
                _context.Vehicles.Remove(vehicle);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> VehicleExists(int id)
        {
            var userVehicles = await Vehicle.GetUserVehicles(_context, _userManager, User);
            return (userVehicles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
