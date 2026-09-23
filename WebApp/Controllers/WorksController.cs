using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    // CRUD de obras. La vista Index incluye filtros de búsqueda
    // (nombre, cliente, estado, rango de fechas y montos).
    public class WorksController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WorksController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Works
        // Aplica los filtros opcionales del WorkFilterViewModel.
        public async Task<IActionResult> Index(WorkFilterViewModel filter)
        {
            var query = _db.Works.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(w => w.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.Client))
                query = query.Where(w => w.Client != null && w.Client.Contains(filter.Client));

            if (filter.Status.HasValue)
                query = query.Where(w => w.Status == filter.Status.Value);

            if (filter.StartDateFrom.HasValue)
                query = query.Where(w => w.StartDate >= filter.StartDateFrom.Value);

            if (filter.StartDateTo.HasValue)
                query = query.Where(w => w.StartDate <= filter.StartDateTo.Value);

            if (filter.EndDateFrom.HasValue)
                query = query.Where(w => w.EndDate.HasValue && w.EndDate >= filter.EndDateFrom.Value);

            if (filter.EndDateTo.HasValue)
                query = query.Where(w => w.EndDate.HasValue && w.EndDate <= filter.EndDateTo.Value);

            if (filter.ContractAmountMin.HasValue)
                query = query.Where(w => w.ContractAmount >= filter.ContractAmountMin.Value);

            if (filter.ContractAmountMax.HasValue)
                query = query.Where(w => w.ContractAmount <= filter.ContractAmountMax.Value);

            filter.Results = await query
                .OrderByDescending(w => w.StartDate)
                .ToListAsync();

            return View(filter);
        }

        // GET: Works/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Se cargan trabajadores y sus pagos para calcular totales
            // por asignación en la vista.
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
        public IActionResult Create() => View();

        // POST: Works/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Client,StartDate,EndDate,ContractAmount,CollectedAmount,Status,Notes")] Work work)
        {
            if (!ModelState.IsValid) return View(work);

            _db.Works.Add(work);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            if (!ModelState.IsValid) return View(work);

            try
            {
                _db.Works.Update(work);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkExists(work.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Works/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var work = await _db.Works.FirstOrDefaultAsync(m => m.Id == id);
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
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool WorkExists(int id) => _db.Works.Any(e => e.Id == id);
    }
}