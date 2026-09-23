using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    // CRUD de trabajadores. La vista Index incluye filtros de búsqueda.
    public class WorkersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WorkersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Workers
        // Aplica los filtros opcionales del WorkerFilterViewModel.
        public async Task<IActionResult> Index(WorkerFilterViewModel filter)
        {
            var query = _db.Workers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(w => w.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.Document))
                query = query.Where(w => w.Document != null && w.Document.Contains(filter.Document));

            if (!string.IsNullOrWhiteSpace(filter.Phone))
                query = query.Where(w => w.Phone != null && w.Phone.Contains(filter.Phone));

            if (!string.IsNullOrWhiteSpace(filter.Bank))
                query = query.Where(w => w.Bank != null && w.Bank.Contains(filter.Bank));

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(w => w.Status == filter.Status);

            filter.Results = await query
                .OrderBy(w => w.Name)
                .ToListAsync();

            return View(filter);
        }

        // GET: Workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Se incluyen los pagos de cada asignación para calcular
            // el total pendiente del trabajador en la vista.
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
        public IActionResult Create() => View();

        // POST: Workers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Document,Phone,Bank,AccountNumber,Status")] Worker worker)
        {
            if (!ModelState.IsValid) return View(worker);

            _db.Workers.Add(worker);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            if (!ModelState.IsValid) return View(worker);

            try
            {
                _db.Workers.Update(worker);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkerExists(worker.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Workers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var worker = await _db.Workers.FirstOrDefaultAsync(m => m.Id == id);
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
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool WorkerExists(int id) => _db.Workers.Any(e => e.Id == id);
    }
}