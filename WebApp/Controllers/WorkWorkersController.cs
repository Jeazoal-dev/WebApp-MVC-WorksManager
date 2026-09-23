using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    // CRUD de asignaciones (WorkWorker) que vinculan obras con trabajadores.
    // La vista Index incluye filtros y calcula el estado de pago por asignación.
    public class WorkWorkersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WorkWorkersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: WorkWorkers
        // Aplica los filtros del WorkWorkerFilterViewModel. Se incluye Payments
        // para poder calcular cuánto se ha pagado en cada fila de la vista.
        public async Task<IActionResult> Index(WorkWorkerFilterViewModel filter)
        {
            var query = _db.WorkWorkers
                .Include(w => w.Work)
                .Include(w => w.Worker)
                .Include(w => w.Payments)
                .AsNoTracking()
                .AsQueryable();

            if (filter.WorkId.HasValue)
                query = query.Where(w => w.WorkId == filter.WorkId.Value);

            if (filter.WorkerId.HasValue)
                query = query.Where(w => w.WorkerId == filter.WorkerId.Value);

            if (filter.AgreedAmountMin.HasValue)
                query = query.Where(w => w.AgreedAmount >= filter.AgreedAmountMin.Value);

            if (filter.AgreedAmountMax.HasValue)
                query = query.Where(w => w.AgreedAmount <= filter.AgreedAmountMax.Value);

            // Listas para los dropdowns del panel de filtros.
            filter.Works = await _db.Works.AsNoTracking().OrderBy(w => w.Name).ToListAsync();
            filter.Workers = await _db.Workers.AsNoTracking().OrderBy(w => w.Name).ToListAsync();

            filter.Results = await query
                .OrderBy(w => w.Work!.Name)
                .ThenBy(w => w.Worker!.Name)
                .ToListAsync();

            return View(filter);
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
            PopulateDropDowns();
            return View();
        }

        // POST: WorkWorkers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkId,WorkerId,AgreedAmount")] WorkWorker workWorker)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropDowns(workWorker.WorkId, workWorker.WorkerId);
                return View(workWorker);
            }

            _db.WorkWorkers.Add(workWorker);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: WorkWorkers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var workWorker = await _db.WorkWorkers.FindAsync(id);
            if (workWorker == null) return NotFound();

            PopulateDropDowns(workWorker.WorkId, workWorker.WorkerId);
            return View(workWorker);
        }

        // POST: WorkWorkers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkId,WorkerId,AgreedAmount")] WorkWorker workWorker)
        {
            if (id != workWorker.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(workWorker.WorkId, workWorker.WorkerId);
                return View(workWorker);
            }

            try
            {
                _db.WorkWorkers.Update(workWorker);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkWorkerExists(workWorker.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
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
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool WorkWorkerExists(int id) => _db.WorkWorkers.Any(e => e.Id == id);

        // Carga los SelectList de obras y trabajadores activos para los
        // formularios de Create y Edit. Los parámetros permiten preseleccionar
        // un valor cuando hay un error de validación o al editar.
        private void PopulateDropDowns(int? selectedWorkId = null, int? selectedWorkerId = null)
        {
            ViewData["WorkId"] = new SelectList(_db.Works, "Id", "Name", selectedWorkId);
            ViewData["WorkerId"] = new SelectList(
                _db.Workers.Where(w => w.Status == "Active"),
                "Id", "Name", selectedWorkerId);
        }
    }
}