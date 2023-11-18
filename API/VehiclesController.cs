using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using CarPark.APIModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace CarPark.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        readonly int _userId;

        public VehiclesController
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

        // GET: api/Vehicles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleAPIModel>>> GetVehicles()
        {
            if (_context.Vehicles == null)
                return NotFound();

            var userVehicles = Vehicle.GetUserVehicles(_context, User, _userId);

            return await userVehicles
                .Include(v => v.DriversVehicles)
                .Select(v => new VehicleAPIModel(v))
                .ToListAsync();
        }

        // GET: api/Vehicles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleAPIModel>> GetVehicle(int id)
        {
            if (_context.Vehicles == null)
                return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.DriversVehicles)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
                return NotFound();

            if (!(await Enterprise.CheckAccess(vehicle.EnterpriseId, _context, User, _userId)))
                return StatusCode(StatusCodes.Status403Forbidden);

            return new VehicleAPIModel(vehicle);
        }

        // PUT: api/Vehicles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicle(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id)
                return BadRequest();

            var prevVehicle = await _context.Vehicles.FindAsync(id);

            if (prevVehicle is null)
                return NotFound();

            var hasAccessToPrevVehicle = await Enterprise.CheckAccess(prevVehicle.EnterpriseId, _context, User, _userId);
            var hasAccessToVehicle = await Enterprise.CheckAccess(vehicle.EnterpriseId, _context, User, _userId);

            if (!hasAccessToPrevVehicle || !hasAccessToVehicle)
                return StatusCode(StatusCodes.Status403Forbidden);

            _context.Entry(vehicle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Vehicles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vehicle>> PostVehicle(Vehicle vehicle)
        {
            if (_context.Vehicles == null)
                return Problem("Entity set 'AppDbContext.Vehicles'  is null.");

            if (!(await Enterprise.CheckAccess(vehicle.EnterpriseId, _context, User, _userId)))
                return StatusCode(StatusCodes.Status403Forbidden);

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVehicle", new { id = vehicle.Id }, new VehicleAPIModel(vehicle));
        }

        // DELETE: api/Vehicles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            if (_context.Vehicles == null)
                return NotFound();

            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
                return NotFound();

            if (!(await Enterprise.CheckAccess(vehicle.EnterpriseId, _context, User, _userId)))
                return StatusCode(StatusCodes.Status403Forbidden);

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool VehicleExists(int id)
        {
            return (_context.Vehicles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
