using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class WorkWorkersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WorkWorkersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: WorkWorkers
        public async Task<IActionResult> Index()
        {
            var workWorkers = _db.WorkWorkers
                .Include(w => w.Work)
                .Include(w => w.Worker);

            return View(await workWorkers.ToListAsync());
        }

        // GET: WorkWorkers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var workWorker = await _db.WorkWorkers
                .Include(w => w.Work)
                .Include(w => w.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workWorker == null) return NotFound();

            return View(workWorker);
        }

        // GET: WorkWorkers/Create
        public IActionResult Create()
        {
            ViewData["WorkId"] = new SelectList(_db.Works, "Id", "Name");
            ViewData["WorkerId"] = new SelectList(_db.Workers.Where(w => w.Status == "Active"), "Id", "Name");
            return View();
        }

        // POST: WorkWorkers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkId,WorkerId,AgreedAmount")] WorkWorker workWorker)
        {
            if (ModelState.IsValid)
            {
                _db.Add(workWorker);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkId"] = new SelectList(_db.Works, "Id", "Name", workWorker.WorkId);
            ViewData["WorkerId"] = new SelectList(_db.Workers.Where(w => w.Status == "Active"), "Id", "Name", workWorker.WorkerId);
            return View(workWorker);
        }

        // GET: WorkWorkers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var workWorker = await _db.WorkWorkers.FindAsync(id);
            if (workWorker == null) return NotFound();

            ViewData["WorkId"] = new SelectList(_db.Works, "Id", "Name", workWorker.WorkId);
            ViewData["WorkerId"] = new SelectList(_db.Workers.Where(w => w.Status == "Active"), "Id", "Name", workWorker.WorkerId);
            return View(workWorker);
        }

        // POST: WorkWorkers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkId,WorkerId,AgreedAmount")] WorkWorker workWorker)
        {
            if (id != workWorker.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(workWorker);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkWorkerExists(workWorker.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkId"] = new SelectList(_db.Works, "Id", "Name", workWorker.WorkId);
            ViewData["WorkerId"] = new SelectList(_db.Workers.Where(w => w.Status == "Active"), "Id", "Name", workWorker.WorkerId);
            return View(workWorker);
        }

        // GET: WorkWorkers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var workWorker = await _db.WorkWorkers
                .Include(w => w.Work)
                .Include(w => w.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workWorker == null) return NotFound();

            return View(workWorker);
        }

        // POST: WorkWorkers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workWorker = await _db.WorkWorkers.FindAsync(id);
            if (workWorker != null)
            {
                _db.WorkWorkers.Remove(workWorker);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkWorkerExists(int id)
        {
            return _db.WorkWorkers.Any(e => e.Id == id);
        }
    }
}
