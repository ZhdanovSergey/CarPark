﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Models;
using CarPark.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CarPark.Controllers;

[Authorize]
public class EnterprisesController : Controller
{
    readonly ApplicationDbContext _context;

    public EnterprisesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Enterprises
    public async Task<IActionResult> Index()
    {
        if (_context.Enterprises == null)
            return Problem("Entity set 'AppDbContext.Enterprise'  is null.");

        var userEnterprises = Enterprise.GetUserEnterprises(_context, User);
        return View(await userEnterprises.ToListAsync());
    }

    // GET: Enterprises/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
            return NotFound();

        var enterprise = await Enterprise.GetUserEnterprises(_context, User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (enterprise is null)
            return NotFound();

        return View(enterprise);
    }

    // GET: Enterprises/Create
    public IActionResult Create()
    {
        return View(new EnterpriseViewModel());
    }

    // POST: Enterprises/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name, City, TimeZoneId")] EnterpriseViewModel enterpriseVM)
    {
        if (ModelState.IsValid)
        {
            var enterprise = new Enterprise(enterpriseVM);
            await enterprise.SaveAndBindWithManager(_context, User);
            return RedirectToAction(nameof(Index));
        }

        return View(enterpriseVM);
    }

    // GET: Enterprises/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Enterprises == null)
            return NotFound();

        var userEnterprises = Enterprise.GetUserEnterprises(_context, User);

        var enterprise = await userEnterprises
            .Where(ue => ue.Id == id)
            .FirstOrDefaultAsync();

        if (enterprise == null)
            return NotFound();

        return View(new EnterpriseViewModel(enterprise));
    }

    // POST: Enterprises/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id, Name, City, TimeZoneId")] EnterpriseViewModel enterpriseVM)
    {
        var userEnterprises = Enterprise.GetUserEnterprises(_context, User);

        if (id != enterpriseVM.Id || await userEnterprises.AllAsync(ue => ue.Id != enterpriseVM.Id))
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(new Enterprise(enterpriseVM));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnterpriseExists(enterpriseVM.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(enterpriseVM);
    }

    // GET: Enterprises/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Enterprises == null)
            return NotFound();

        var userEnterprises = Enterprise.GetUserEnterprises(_context, User);

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

        var userEnterprises = Enterprise.GetUserEnterprises(_context, User);

        var enterprise = await userEnterprises
            .Where(ue => ue.Id == id)
            .FirstOrDefaultAsync();

        if (enterprise != null)
            _context.Enterprises.Remove(enterprise);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool EnterpriseExists(int id)
    {
        var userEnterprises = Enterprise.GetUserEnterprises(_context, User);
        return (userEnterprises?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
