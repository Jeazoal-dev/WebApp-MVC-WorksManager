using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PaymentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var payments = _db.Payments
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Work)
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Worker);

            return View(await payments.ToListAsync());
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
        public IActionResult Create(int? workWorkerId)
        {
            var workWorkers = _db.WorkWorkers
                .Include(ww => ww.Work)
                .Include(ww => ww.Worker)
                .Select(ww => new
                {
                    ww.Id,
                    Display = ww.Work!.Name + " - " + ww.Worker!.Name
                });

            ViewData["WorkWorkerId"] = new SelectList(workWorkers, "Id", "Display", workWorkerId);
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkWorkerId,Amount,Date,PaymentMethod,Bank,Reference,UserId,Cancelled,Notes")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _db.Add(payment);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var workWorkers = _db.WorkWorkers
                .Include(ww => ww.Work)
                .Include(ww => ww.Worker)
                .Select(ww => new
                {
                    ww.Id,
                    Display = ww.Work!.Name + " - " + ww.Worker!.Name
                });

            ViewData["WorkWorkerId"] = new SelectList(workWorkers, "Id", "Display", payment.WorkWorkerId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _db.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            var workWorkers = _db.WorkWorkers
                .Include(ww => ww.Work)
                .Include(ww => ww.Worker)
                .Select(ww => new
                {
                    ww.Id,
                    Display = ww.Work!.Name + " - " + ww.Worker!.Name
                });

            ViewData["WorkWorkerId"] = new SelectList(workWorkers, "Id", "Display", payment.WorkWorkerId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkWorkerId,Amount,Date,PaymentMethod,Bank,Reference,UserId,Cancelled,Notes")] Payment payment)
        {
            if (id != payment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(payment);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var workWorkers = _db.WorkWorkers
                .Include(ww => ww.Work)
                .Include(ww => ww.Worker)
                .Select(ww => new
                {
                    ww.Id,
                    Display = ww.Work!.Name + " - " + ww.Worker!.Name
                });

            ViewData["WorkWorkerId"] = new SelectList(workWorkers, "Id", "Display", payment.WorkWorkerId);
            return View(payment);
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
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _db.Payments.Any(e => e.Id == id);
        }
    }
}
