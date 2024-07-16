using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PieShop.Models;

namespace PieShop.Areas.Admin
{
    [Area("Admin")]
    public class Pies : Controller
    {
        private readonly PieShopDbContext _context;

        public Pies(PieShopDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Pies
        public async Task<IActionResult> Index()
        {
            var pieShopDbContext = _context.pies.Include(p => p.Category);
            return View(await pieShopDbContext.ToListAsync());
        }

        // GET: Admin/Pies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pie = await _context.pies
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PieId == id);
            if (pie == null)
            {
                return NotFound();
            }

            return View(pie);
        }

        // GET: Admin/Pies/Create
        public IActionResult Create()
        {
            ViewData["Category"] = new SelectList(_context.categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Admin/Pies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PieId,Name,ShortDescription,LongDescription,AllergyInformation,Price,ImageUrl,ImageThumbnailUrl,IsPieOfTheWeek,InStock,CategoryId")] Pie pie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Category = new SelectList(_context.categories, "CategoryId", "CategoryName", pie.CategoryId);
            return View(pie);
        }

        // GET: Admin/Pies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pie = await _context.pies.FindAsync(id);
            if (pie == null)
            {
                return NotFound();
            }
            ViewBag.Category = new SelectList(_context.categories, "CategoryId", "CategoryName", pie.CategoryId);
            return View(pie);
        }

        // POST: Admin/Pies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PieId,Name,ShortDescription,LongDescription,AllergyInformation,Price,ImageUrl,ImageThumbnailUrl,IsPieOfTheWeek,InStock,CategoryId")] Pie pie)
        {
            if (id != pie.PieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PieExists(pie.PieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Category = new SelectList(_context.categories, "CategoryId", "CategoryName", pie.CategoryId);
            return View(pie);
        }

        // GET: Admin/Pies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pie = await _context.pies
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PieId == id);
            if (pie == null)
            {
                return NotFound();
            }

            return View(pie);
        }

        // POST: Admin/Pies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pie = await _context.pies.FindAsync(id);
            if (pie != null)
            {
                _context.pies.Remove(pie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PieExists(int id)
        {
            return _context.pies.Any(e => e.PieId == id);
        }
    }
}
