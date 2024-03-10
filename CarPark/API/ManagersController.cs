using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using CarPark.APIModels;

namespace CarPark.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleNames.Admin)]
    public class ManagersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ManagersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Managers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagerAPIModel>>> GetManagers()
        {
            if (_context.Managers == null)
                return NotFound();

            return await _context.Managers
                .Include(m => m.EnterprisesManagers)
                .Select(m => new ManagerAPIModel(m))
                .ToListAsync();
        }

        // GET: api/Managers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagerAPIModel>> GetManager(int id)
        {
            if (_context.Managers == null)
                return NotFound();

            var manager = await _context.Managers
                .Include(m => m.EnterprisesManagers)
                .Select(m => new ManagerAPIModel(m))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null)
                return NotFound();

            return manager;
        }

        // PUT: api/Managers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManager(int id, Manager manager)
        {
            if (id != manager.Id)
            {
                return BadRequest();
            }

            _context.Entry(manager).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Managers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Manager>> PostManager(Manager manager)
        {
          if (_context.Managers == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Managers'  is null.");
          }
            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetManager", new { id = manager.Id }, manager);
        }

        // DELETE: api/Managers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManager(int id)
        {
            if (_context.Managers == null)
            {
                return NotFound();
            }
            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ManagerExists(int id)
        {
            return (_context.Managers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
