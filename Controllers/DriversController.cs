using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using CarPark.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Drawing.Printing;

namespace CarPark.Controllers
{
    [Authorize]
    public class DriversController : Controller
    {
        private readonly ApplicationDbContext _context;
        readonly int _userId;

        public DriversController
        (
            ApplicationDbContext context,
            IHttpContextAccessor contextAccessor,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;

            var user = contextAccessor.HttpContext?.User;
            string? userId = user is not null ? userManager.GetUserId(user) : null;
            _userId = Int32.Parse(userId ?? "");
        }

        // GET: Drivers
        public async Task<IActionResult> Index(int page = 1)
        {
            const int PAGE_SIZE = 20;

            if (_context.Drivers == null)
                return Problem("Entity set 'AppDbContext.Drivers'  is null.");

            var userDrivers = Driver.GetUserDrivers(_context, User, _userId)
                .Include(d => d.ActiveVehicle)
                .Include(d => d.Enterprise);

            var driversWithpagination = new Pagination<Driver>(userDrivers, PAGE_SIZE, page);
            return View(driversWithpagination);
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Drivers == null)
                return NotFound();

            var userDrivers = Driver.GetUserDrivers(_context, User, _userId);

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
        public async Task<IActionResult> Create()
        {
            var driverVM = new DriverViewModel();
            driverVM.AddSelectLists(_context, User, _userId);
            return View(driverVM);
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Salary,EnterpriseId")] DriverViewModel driverVM)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new Driver(driverVM));
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            driverVM.AddSelectLists(_context, User, _userId);
            return View(driverVM);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Drivers == null)
                return NotFound();

            var userDrivers = Driver.GetUserDrivers(_context, User, _userId);

            var driver = await userDrivers
                .Include(d => d.DriversVehicles)
                .Include(d => d.ActiveVehicle)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null)
                return NotFound();

            var driverVM = new DriverViewModel(driver);
            driverVM.AddSelectLists(_context, User, _userId);

            return View(driverVM);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Salary,EnterpriseId,VehiclesIds,ActiveVehicleId")] DriverViewModel driverVM)
        {
            var userDrivers = Driver.GetUserDrivers(_context, User, _userId);
            var userEnterprises = Enterprise.GetUserEnterprises(_context, User, _userId);

            if
            (
                id != driverVM.Id
                || await userDrivers.AllAsync(ud => ud.Id != driverVM.Id)
                || await userEnterprises.AllAsync(ue => ue.Id != driverVM.EnterpriseId)
            )
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldDriversVehicles = await _context.DriversVehicles
                        .Where(dv => dv.DriverId == driverVM.Id)
                        .ToListAsync();

                    var newDriversVehicles = await _context.Vehicles
                        .Where(vehicle =>
                            vehicle.EnterpriseId == driverVM.EnterpriseId
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
                        await Vehicle.RemoveActiveDriver(_context, driverVM.Id);

                        var activeVehicle = await _context.Vehicles
                            .FirstOrDefaultAsync(v => v.Id == driverVM.ActiveVehicleId);

                        if (activeVehicle != null)
                        {
                            activeVehicle.ActiveDriverId = driverVM.Id;
                            _context.Update(activeVehicle);
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

            driverVM.AddSelectLists(_context, User, _userId);
            return View(driverVM);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Drivers == null)
                return NotFound();

            var userDrivers = Driver.GetUserDrivers(_context, User, _userId);

            var driver = await userDrivers
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

            var userDrivers = Driver.GetUserDrivers(_context, User, _userId);

            var driver = await userDrivers.FirstOrDefaultAsync(ud => ud.Id == id);

            if (driver != null)
                _context.Drivers.Remove(driver);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            var userDrivers = Driver.GetUserDrivers(_context, User, _userId);
            return (userDrivers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
