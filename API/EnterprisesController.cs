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
using System.Security.Claims;

namespace CarPark.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnterprisesController : ControllerBase
    {
        readonly ApplicationDbContext _context;
        readonly int _userId;

        public EnterprisesController
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

        // GET: api/Enterprises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnterpriseAPIModel>>> GetEnterprises()
        {
            if (_context.Enterprises == null)
                return NotFound();

            var userEnterprises = Enterprise.GetUserEnterprises(_context, User, _userId);

            return await userEnterprises
                .Include(e => e.Drivers)
                    .ThenInclude(d => d.ActiveVehicle)
                .Include(e => e.Vehicles)
                .Select(e => new EnterpriseAPIModel(e))
                .ToListAsync();
        }

        // GET: api/Enterprises/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EnterpriseAPIModel>> GetEnterprise(int id)
        {
            if (_context.Enterprises == null)
                return NotFound();

            var enterprise = await _context.Enterprises
                .Include(e => e.Drivers)
                    .ThenInclude(d => d.ActiveVehicle)
                .Include(e => e.Vehicles)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enterprise == null)
                return NotFound();

            if (!(await Enterprise.CheckAccess(id, _context, User, _userId)))
                return StatusCode(StatusCodes.Status403Forbidden);

            return new EnterpriseAPIModel(enterprise);
        }

        // PUT: api/Enterprises/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnterprise(int id, Enterprise enterprise)
        {
            if (id != enterprise.Id)
                return BadRequest();

            if (!(await Enterprise.CheckAccess(id, _context, User, _userId)))
                return StatusCode(StatusCodes.Status403Forbidden);

            _context.Entry(enterprise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnterpriseExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Enterprises
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Enterprise>> PostEnterprise(Enterprise enterprise)
        {
            if (_context.Enterprises == null)
                return Problem("Entity set 'AppDbContext.Enterprises'  is null.");

            await enterprise.SaveAndBindWithManager(_context, User, _userId);
            return CreatedAtAction("GetEnterprise", new { id = enterprise.Id }, new EnterpriseAPIModel(enterprise));
        }

        // DELETE: api/Enterprises/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnterprise(int id)
        {
            if (_context.Enterprises == null)
                return NotFound();

            var enterprise = await _context.Enterprises.FindAsync(id);

            if (enterprise == null)
                return NotFound();

            if (!(await Enterprise.CheckAccess(id, _context, User, _userId)))
                return StatusCode(StatusCodes.Status403Forbidden);

            _context.Enterprises.Remove(enterprise);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool EnterpriseExists(int id)
        {
            return (_context.Enterprises?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
