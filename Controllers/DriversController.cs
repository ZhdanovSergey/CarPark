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
    public class DriversController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly UserManager<ApplicationUser> _userManager;

        public DriversController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        async Task<IQueryable<Driver>> GetUserDrivers()
        {
            if (User.IsInRole(RoleNames.Admin))
                return _context.Drivers;

            if (User.IsInRole(RoleNames.Manager))
            {
                var managerId = (await _userManager.FindByNameAsync(User.Identity.Name))?.Id;

                return _context.Drivers
                        .Include(d => d.Enterprise)
                            .ThenInclude(e => e.EnterprisesManagers)
                        .Where(d => d.Enterprise.EnterprisesManagers.Any(em => em.ManagerId == managerId));
            }

            throw new Exception($"User role should be {RoleNames.Admin} or {RoleNames.Manager}");
        }

        // GET: Drivers
        public async Task<IActionResult> Index()
        {
            var userDrivers = await GetUserDrivers();

            var drivers = await userDrivers
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

            var userDrivers = await GetUserDrivers();

            var driver = await userDrivers
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
        [Authorize(Roles = RoleNames.Admin)]
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
        [Authorize(Roles = RoleNames.Admin)]
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
        [Authorize(Roles = RoleNames.Admin)]
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
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Salary,EnterpriseId,VehiclesIds,ActiveVehicleId")] DriverViewModel driverVM)
        {
            if (id != driverVM.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var oldDriversVehicles = await _context.DriversVehicles
                        .Where(dv => dv.DriverId == driverVM.Id)
                        .ToListAsync();

                    var newDriversVehicles = await _context.Vehicles
                        .Where(vehicle => vehicle.EnterpriseId == driverVM.EnterpriseId
                            && driverVM.VehiclesIds.Any(vid => vid == vehicle.Id))
                        .Select(vehicle => new DriverVehicle
                        {
                            EnterpriseId = driverVM.EnterpriseId,
                            DriverId = driverVM.Id,
                            VehicleId = vehicle.Id,
                        }).ToListAsync();

                    DriverVehicle.Update(_context, oldDriversVehicles, newDriversVehicles, DriverVehicleIdProp.VehicleId);

                    if (newDriversVehicles.Any(dv => dv.VehicleId == driverVM.ActiveVehicleId))
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
        [Authorize(Roles = RoleNames.Admin)]
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
        [Authorize(Roles = RoleNames.Admin)]
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
