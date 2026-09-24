using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Models;

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
            // KPIs
            ViewBag.ActiveWorks = await _db.Works.CountAsync(w => w.Status == WorkStatus.InProgress);
            ViewBag.TotalWorks = await _db.Works.CountAsync();
            ViewBag.ActiveWorkers = await _db.Workers.CountAsync(w => w.Status == "Active");

            ViewBag.TotalPaid = await _db.Payments
                .Where(p => !p.Cancelled)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            // Pendiente = suma de AgreedAmount - suma de Payments por asignación
            var pendings = await _db.WorkWorkers
                .Select(ww => new
                {
                    ww.AgreedAmount,
                    Paid = ww.Payments.Where(p => !p.Cancelled).Sum(p => (decimal?)p.Amount) ?? 0m
                })
                .ToListAsync();

            ViewBag.TotalPending = pendings.Sum(x => x.AgreedAmount - x.Paid);

            // Panel "Obras activas"
            ViewBag.ActiveWorksList = await _db.Works
                .Where(w => w.Status == WorkStatus.InProgress)
                .Select(w => new
                {
                    w.Id,
                    w.Name,
                    Agreed = w.WorkWorkers.Sum(ww => ww.AgreedAmount),
                    Paid = w.WorkWorkers.SelectMany(ww => ww.Payments)
                                        .Where(p => !p.Cancelled)
                                        .Sum(p => (decimal?)p.Amount) ?? 0m,
                    Pending = w.WorkWorkers.Sum(ww => ww.AgreedAmount)
                              - (w.WorkWorkers.SelectMany(ww => ww.Payments)
                                              .Where(p => !p.Cancelled)
                                              .Sum(p => (decimal?)p.Amount) ?? 0m)
                })
                .OrderByDescending(w => w.Agreed)
                .ToListAsync();

            // Panel "Pagos recientes"
            ViewBag.RecentPayments = await _db.Payments
                .Where(p => !p.Cancelled)
                .OrderByDescending(p => p.Date)
                .Take(5)
                .Select(p => new
                {
                    p.Date,
                    Worker = p.WorkWorker!.Worker!.Name,
                    Work = p.WorkWorker!.Work!.Name,
                    p.Amount
                })
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
