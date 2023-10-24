using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace CarPark.Controllers
{
    [Authorize]
    public class EnterprisesController : Controller
    {
        readonly ApplicationDbContext _context;
        readonly UserManager<ApplicationUser> _userManager;

        public EnterprisesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        async Task<IQueryable<Enterprise>> GetUserEnterprises()
        {
            if (User.IsInRole(RoleNames.Admin))
                return _context.Enterprises;

            if (User.IsInRole(RoleNames.Manager))
            {
                var managerId = (await _userManager.FindByNameAsync(User.Identity.Name))?.Id;

                return _context.Enterprises
                        .Include(e => e.EnterprisesManagers)
                        .Where(e => e.EnterprisesManagers.Any(em => em.ManagerId == managerId));
            }

            throw new Exception($"User role should be {RoleNames.Admin} or {RoleNames.Manager}");
        }

        // GET: Enterprises
        public async Task<IActionResult> Index()
        {
            if (_context.Enterprises == null)
                return Problem("Entity set 'AppDbContext.Enterprise'  is null.");

            var userEnterprises = await GetUserEnterprises();

            return View(await userEnterprises
                .Include(e => e.Drivers)
                .Include(e => e.Vehicles)
                .ToListAsync());
        }

        // GET: Enterprises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Enterprises == null)
                return NotFound();

            var userEnterprises = await GetUserEnterprises();

            var enterprise = await userEnterprises
                .Include(e => e.Drivers)
                .Include(e => e.Vehicles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enterprise == null)
                return NotFound();

            return View(enterprise);
        }

        // GET: Enterprises/Create
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Enterprises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Create([Bind("Id,Name,City")] Enterprise enterprise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enterprise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(enterprise);
        }

        // GET: Enterprises/Edit/5
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enterprises == null)
                return NotFound();

            var enterprise = await _context.Enterprises.FindAsync(id);

            if (enterprise == null)
                return NotFound();

            return View(enterprise);
        }

        // POST: Enterprises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,City")] Enterprise enterprise)
        {
            if (id != enterprise.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enterprise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnterpriseExists(enterprise.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(enterprise);
        }

        // GET: Enterprises/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enterprises == null)
                return NotFound();

            var enterprise = await _context.Enterprises
                .Include(e => e.Drivers)
                .Include(e => e.Vehicles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enterprise == null)
                return NotFound();

            return View(enterprise);
        }

        // POST: Enterprises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enterprises == null)
                return Problem("Entity set 'AppDbContext.Enterprise'  is null.");

            var enterprise = await _context.Enterprises.FindAsync(id);

            if (enterprise != null)
                _context.Enterprises.Remove(enterprise);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EnterpriseExists(int id)
        {
          return (_context.Enterprises?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
