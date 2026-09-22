using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Totales generales
            ViewBag.TotalWorks = await _db.Works.CountAsync();
            ViewBag.ActiveWorks = await _db.Works.CountAsync(w => w.Status == "In progress");
            ViewBag.TotalWorkers = await _db.Workers.CountAsync(w => w.Status == "Active");

            ViewBag.TotalAgreed = await _db.WorkWorkers.SumAsync(ww => (decimal?)ww.AgreedAmount) ?? 0;
            ViewBag.TotalPaid = await _db.Payments
                .Where(p => !p.Cancelled)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;
            ViewBag.TotalPending = ViewBag.TotalAgreed - ViewBag.TotalPaid;

            // Últimos 5 pagos
            ViewBag.RecentPayments = await _db.Payments
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Work)
                .Include(p => p.WorkWorker)
                    .ThenInclude(ww => ww.Worker)
                .OrderByDescending(p => p.Date)
                .Take(5)
                .ToListAsync();

            // Obras activas con su info
            ViewBag.ActiveWorksList = await _db.Works
                .Include(w => w.WorkWorkers)
                    .ThenInclude(ww => ww.Payments)
                .Where(w => w.Status == "In progress")
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
