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
    public class DriversController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriversController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Drivers
        [HttpGet]
        public async Task<ActionResult<Pagination<DriverAPIModel>>> GetDrivers(int page = 1, int pageSize = 20, int enterpriseId = 0)
        {
            if (_context.Drivers is null)
                return NotFound();

            var userDrivers = Driver.GetUserDrivers(_context, User)
                .Where(v => enterpriseId == 0 || v.EnterpriseId == enterpriseId)
                .Include(d => d.ActiveVehicle)
                .Include(d => d.DriversVehicles)
                .Select(d => new DriverAPIModel(d));

            return await Pagination<DriverAPIModel>.PaginationAsync(userDrivers, page, pageSize);
        }

        // GET: api/Drivers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DriverAPIModel>> GetDriver(int id)
        {
            if (_context.Drivers == null)
                return NotFound();

            var driver = await _context.Drivers
                .Include(d => d.ActiveVehicle)
                .Include(d => d.DriversVehicles)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                return NotFound();

            if (!(await Enterprise.CheckAccess(driver.EnterpriseId, _context, User)))
                return StatusCode(StatusCodes.Status403Forbidden);

            return new DriverAPIModel(driver);
        }

        // PUT: api/Drivers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriver(int id, Driver driver)
        {
            if (id != driver.Id)
                return BadRequest();

            var prevDriver = await _context.Drivers.FindAsync(id);

            if (prevDriver is null)
                return NotFound();

            var hasAccessToPrevDriver = await Enterprise.CheckAccess(prevDriver.EnterpriseId, _context, User);
            var hasAccessToDriver = await Enterprise.CheckAccess(driver.EnterpriseId, _context, User);

            if (!hasAccessToPrevDriver || !hasAccessToDriver)
                return StatusCode(StatusCodes.Status403Forbidden);

            _context.Entry(driver).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriverExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Drivers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Driver>> PostDriver(Driver driver)
        {
            if (_context.Drivers == null)
                return Problem("Entity set 'AppDbContext.Drivers'  is null.");

            if (!(await Enterprise.CheckAccess(driver.EnterpriseId, _context, User)))
                return StatusCode(StatusCodes.Status403Forbidden);

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDriver", new { id = driver.Id }, new DriverAPIModel(driver));
        }

        // DELETE: api/Drivers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            if (_context.Drivers == null)
                return NotFound();

            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound();

            if (!(await Enterprise.CheckAccess(driver.EnterpriseId, _context, User)))
                return StatusCode(StatusCodes.Status403Forbidden);

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool DriverExists(int id)
        {
            return (_context.Drivers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
