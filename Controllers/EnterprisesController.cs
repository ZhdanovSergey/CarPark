using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using NuGet.Versioning;

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

        // GET: Enterprises
        public async Task<IActionResult> Index()
        {
            if (_context.Enterprises == null)
                return Problem("Entity set 'AppDbContext.Enterprise'  is null.");

            var userEnterprises = await Enterprise.GetUserEnterprises(_context, _userManager, User);

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

            var userEnterprises = await Enterprise.GetUserEnterprises(_context, _userManager, User);

            var enterprise = await userEnterprises
                .Include(e => e.Drivers)
                .Include(e => e.Vehicles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enterprise == null)
                return NotFound();

            return View(enterprise);
        }

        // GET: Enterprises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Enterprises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,City")] Enterprise enterprise)
        {
            if (ModelState.IsValid)
            {
                await Enterprise.SaveAndBindWithManager(_context, _userManager, User, enterprise);
                return RedirectToAction(nameof(Index));
            }

            return View(enterprise);
        }

        // GET: Enterprises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enterprises == null)
                return NotFound();

            var userEnterprises = await Enterprise.GetUserEnterprises(_context, _userManager, User);

            var enterprise = await userEnterprises
                .Where(ue => ue.Id == id)
                .FirstOrDefaultAsync();

            if (enterprise == null)
                return NotFound();

            return View(enterprise);
        }

        // POST: Enterprises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,City")] Enterprise enterprise)
        {
            var userEnterprises = await Enterprise.GetUserEnterprises(_context, _userManager, User);

            if (id != enterprise.Id || await userEnterprises.AllAsync(ue => ue.Id != enterprise.Id))
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
                    if (!(await EnterpriseExists(enterprise.Id)))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(enterprise);
        }

        // GET: Enterprises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enterprises == null)
                return NotFound();


            var userEnterprises = await Enterprise.GetUserEnterprises(_context, _userManager, User);

            var enterprise = await userEnterprises
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enterprises == null)
                return Problem("Entity set 'AppDbContext.Enterprise'  is null.");

            var userEnterprises = await Enterprise.GetUserEnterprises(_context, _userManager, User);

            var enterprise = await userEnterprises
                .Where(ue => ue.Id == id)
                .FirstOrDefaultAsync();

            if (enterprise != null)
                _context.Enterprises.Remove(enterprise);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EnterpriseExists(int id)
        {
            var userEnterprises = await Enterprise.GetUserEnterprises(_context, _userManager, User);
            return (userEnterprises?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
