using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class WorksController: Controller
    {
        private readonly ApplicationDbContext _db;

        public WorksController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Works
        public async Task<IActionResult> Index()
        {
            return View(await _db.Works.ToListAsync());
        }

        // GET: Works/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var work = await _db.Works
                .Include(w => w.WorkWorkers)
                    .ThenInclude(ww => ww.Worker)
                .Include(w => w.WorkWorkers)
                    .ThenInclude(ww => ww.Payments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (work == null) return NotFound();

            return View(work);
        }

        // GET: Works/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Works/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Client,StartDate,EndDate,ContractAmount,CollectedAmount,Status,Notes")] Work work)
        {
            if (ModelState.IsValid)
            {
                _db.Add(work);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(work);
        }

        // GET: Works/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var work = await _db.Works.FindAsync(id);
            if (work == null) return NotFound();

            return View(work);
        }

        // POST: Works/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Client,StartDate,EndDate,ContractAmount,CollectedAmount,Status,Notes")] Work work)
        {
            if (id != work.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(work);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkExists(work.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(work);
        }

        // GET: Works/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var work = await _db.Works
                .FirstOrDefaultAsync(m => m.Id == id);

            if (work == null) return NotFound();

            return View(work);
        }

        // POST: Works/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var work = await _db.Works.FindAsync(id);
            if (work != null)
            {
                _db.Works.Remove(work);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkExists(int id)
        {
            return _db.Works.Any(e => e.Id == id);
        }
    }
}
