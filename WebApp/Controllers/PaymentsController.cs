using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    // CRUD de pagos a trabajadores. Cada pago está vinculado a una asignación (WorkWorker),
    // que a su vez relaciona una obra con un trabajador.
    // La vista Index incluye filtros de búsqueda por obra, trabajador, fechas, montos, etc.
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PaymentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Payments
        // Aplica los filtros opcionales del PaymentFilterViewModel y devuelve
        // también las listas de obras y trabajadores para los dropdowns del filtro.
        public async Task<IActionResult> Index(PaymentFilterViewModel filter)
        {
            var query = _db.Payments
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Work)
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Worker)
                .AsNoTracking()
                .AsQueryable();

            if (filter.WorkId.HasValue)
                query = query.Where(p => p.WorkWorker!.WorkId == filter.WorkId.Value);

            if (filter.WorkerId.HasValue)
                query = query.Where(p => p.WorkWorker!.WorkerId == filter.WorkerId.Value);

            if (!string.IsNullOrWhiteSpace(filter.PaymentMethod))
                query = query.Where(p => p.PaymentMethod != null && p.PaymentMethod.Contains(filter.PaymentMethod));

            if (!string.IsNullOrWhiteSpace(filter.Bank))
                query = query.Where(p => p.Bank != null && p.Bank.Contains(filter.Bank));

            if (!string.IsNullOrWhiteSpace(filter.Reference))
                query = query.Where(p => p.Reference != null && p.Reference.Contains(filter.Reference));

            if (filter.DateFrom.HasValue)
                query = query.Where(p => p.Date >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(p => p.Date <= filter.DateTo.Value);

            if (filter.AmountMin.HasValue)
                query = query.Where(p => p.Amount >= filter.AmountMin.Value);

            if (filter.AmountMax.HasValue)
                query = query.Where(p => p.Amount <= filter.AmountMax.Value);

            if (filter.Cancelled.HasValue)
                query = query.Where(p => p.Cancelled == filter.Cancelled.Value);

            // Listas para los dropdowns del panel de filtros.
            filter.Works = await _db.Works.AsNoTracking().OrderBy(w => w.Name).ToListAsync();
            filter.Workers = await _db.Workers.AsNoTracking().OrderBy(w => w.Name).ToListAsync();

            filter.Results = await query
                .OrderByDescending(p => p.Date)
                .ToListAsync();

            return View(filter);
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _db.Payments
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Work)
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        // GET: Payments/Create
        // El parámetro opcional workWorkerId permite precargar la asignación
        // cuando se llega desde "Registrar pago" en otra vista.
        public IActionResult Create(int? workWorkerId)
        {
            PopulateWorkWorkersDropDown(workWorkerId);
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkWorkerId,Amount,Date,PaymentMethod,Bank,Reference,UserId,Cancelled,Notes")] Payment payment)
        {
            if (!ModelState.IsValid)
            {
                PopulateWorkWorkersDropDown(payment.WorkWorkerId);
                return View(payment);
            }

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _db.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            PopulateWorkWorkersDropDown(payment.WorkWorkerId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkWorkerId,Amount,Date,PaymentMethod,Bank,Reference,UserId,Cancelled,Notes")] Payment payment)
        {
            if (id != payment.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateWorkWorkersDropDown(payment.WorkWorkerId);
                return View(payment);
            }

            try
            {
                _db.Payments.Update(payment);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(payment.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _db.Payments
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Work)
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment != null)
            {
                _db.Payments.Remove(payment);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id) => _db.Payments.Any(e => e.Id == id);

        // Carga el SelectList de asignaciones con el formato "Obra - Trabajador"
        // usado por los formularios de Create y Edit.
        private void PopulateWorkWorkersDropDown(int? selectedId = null)
        {
            var workWorkers = _db.WorkWorkers
                .Include(ww => ww.Work)
                .Include(ww => ww.Worker)
                .Select(ww => new
                {
                    ww.Id,
                    Display = ww.Work!.Name + " - " + ww.Worker!.Name
                });

            ViewData["WorkWorkerId"] = new SelectList(workWorkers, "Id", "Display", selectedId);
        }
    }
}