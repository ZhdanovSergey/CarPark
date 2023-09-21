using System;
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
        private readonly AppDbContext _context;

        public VehiclesController(AppDbContext context)
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
                    var driversVehicles = await _context.DriversVehicles
                        .Where(dv => dv.VehicleId == vehicleVM.Id)
                        .ToListAsync();

                    var validSelectedDriversIds = await _context.Drivers
                        .Where(d => vehicleVM.DriversIds.Any(sdid => sdid == d.Id)
                            && d.EnterpriseId == vehicleVM.EnterpriseId)
                        .Select(d => d.Id)
                        .ToListAsync();

                    foreach (var driverVehicle in driversVehicles)
                    {
                        if (!validSelectedDriversIds.Any(id => id == driverVehicle.DriverId))
                            _context.DriversVehicles.Remove(driverVehicle);
                    }

                    foreach (var validSelectedDriverId in validSelectedDriversIds)
                    {
                        if (!driversVehicles.Any(m => m.DriverId == validSelectedDriverId))
                        {
                            _context.DriversVehicles.Add(new DriverVehicle
                            {
                                EnterpriseId = vehicleVM.EnterpriseId,
                                DriverId = validSelectedDriverId,
                                VehicleId = vehicleVM.Id
                            });
                        }
                    }

                    vehicleVM.ActiveDriverId = validSelectedDriversIds.Any(sdid => sdid == vehicleVM.ActiveDriverId)
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
