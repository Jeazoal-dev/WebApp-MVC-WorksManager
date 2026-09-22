using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{

    public class WorkersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const int PageSize = 10;

        public WorkersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Workers
        public async Task<IActionResult> Index(string? search, string? status, int page = 1)
        {
            var query = _db.Workers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(w =>
                    w.Name.Contains(search) ||
                    (w.Document != null && w.Document.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(w => w.Status == status);
            }

            var total = await query.CountAsync();
            var workers = await query
                .OrderBy(w => w.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / PageSize);
            ViewBag.Total = total;

            return View(workers);
        }

        // GET: Workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var worker = await _db.Workers
                .Include(w => w.WorkWorkers)
                    .ThenInclude(ww => ww.Work)
                .Include(w => w.WorkWorkers)
                    .ThenInclude(ww => ww.Payments)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null) return NotFound();

            var vm = new WorkerEditViewModel
            {
                Worker = worker,
                Assignments = worker.WorkWorkers.ToList(),
                TotalWorks = worker.WorkWorkers.Count,
                TotalAgreed = worker.WorkWorkers.Sum(ww => ww.AgreedAmount),
                TotalPaid = worker.WorkWorkers.Sum(ww => ww.Payments.Where(p => !p.Cancelled).Sum(p => p.Amount))
            };
            vm.TotalPending = vm.TotalAgreed - vm.TotalPaid;

            return View(vm);
        }

        // GET: Workers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Document,Phone,Bank,AccountNumber,Status")] Worker worker)
        {
            if (ModelState.IsValid)
            {
                _db.Add(worker);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(worker);
        }

        // GET: Workers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var worker = await _db.Workers.FindAsync(id);
            if (worker == null) return NotFound();

            return View(worker);
        }

        // POST: Workers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Document,Phone,Bank,AccountNumber,Status")] Worker worker)
        {
            if (id != worker.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(worker);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(worker);
        }

        // GET: Workers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var worker = await _db.Workers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (worker == null) return NotFound();

            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worker = await _db.Workers.FindAsync(id);
            if (worker != null)
            {
                _db.Workers.Remove(worker);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkerExists(int id)
        {
            return _db.Workers.Any(e => e.Id == id);
        }
    }
}