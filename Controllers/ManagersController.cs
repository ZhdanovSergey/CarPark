using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using CarPark.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CarPark.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class ManagersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Managers
        public async Task<IActionResult> Index()
        {
            if (_context.Managers == null)
                return Problem("Entity set 'ApplicationDbContext.Managers'  is null.");

            var managers = await _context.Managers
                .Include(m => m.EnterprisesManagers)
                    .ThenInclude(em => em.Enterprise)
                .ToListAsync();

            return View(managers);
        }

        // GET: Managers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Managers == null)
                return NotFound();

            var manager = await _context.Managers
                .Include(m => m.EnterprisesManagers)
                    .ThenInclude(em => em.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null)
                return NotFound();

            return View(manager);
        }

        // GET: Managers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Managers == null)
                return NotFound();

            var manager = await _context.Managers
                .Include(m => m.EnterprisesManagers)
                    .ThenInclude(em => em.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null)
                return NotFound();

            var managerVM = new ManagerViewModel(manager);
            await managerVM.AddEditSelectLists(_context);

            return View(managerVM);
        }

        // POST: Managers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EnterprisesIds")] ManagerViewModel managerVM)
        {
            if (id != managerVM.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var oldEnterprisesManagers = await _context.EnterprisesManagers
                        .Where(em => em.ManagerId == managerVM.Id)
                        .ToListAsync();

                    var newEnterprisesManagers = await _context.Enterprises
                        .Where(enterprise => managerVM.EnterprisesIds.Any(did => did == enterprise.Id))
                        .Select(enterprise => new EnterpriseManager
                        {
                            EnterpriseId = enterprise.Id,
                            ManagerId = managerVM.Id,
                        }).ToListAsync();

                    EnterpriseManager.Update(_context, oldEnterprisesManagers, newEnterprisesManagers, EnterpriseManagerIdProp.EnterpriseId);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManagerExists(managerVM.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }
            return View(managerVM);
        }

        // GET: Managers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Managers == null)
                return NotFound();

            var manager = await _context.Managers
                .Include(m => m.EnterprisesManagers)
                    .ThenInclude(em => em.Enterprise)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null)
                return NotFound();

            return View(manager);
        }

        // POST: Managers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Managers == null)
                return Problem("Entity set 'ApplicationDbContext.Managers'  is null.");

            var manager = await _context.Managers.FindAsync(id);

            if (manager != null)
                _context.Managers.Remove(manager);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ManagerExists(int id)
        {
          return (_context.Managers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
