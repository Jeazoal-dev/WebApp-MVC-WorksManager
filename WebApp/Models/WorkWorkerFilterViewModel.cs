using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class WorkWorkerFilterViewModel
    {
        [Display(Name = "Obra")]
        public int? WorkId { get; set; }

        [Display(Name = "Trabajador")]
        public int? WorkerId { get; set; }

        [Display(Name = "Monto acordado mín.")]
        public decimal? AgreedAmountMin { get; set; }

        [Display(Name = "Monto acordado máx.")]
        public decimal? AgreedAmountMax { get; set; }

        // Para los dropdowns
        public List<Work> Works { get; set; } = new();
        public List<Worker> Workers { get; set; } = new();

        // Paginación
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public List<WorkWorker> Results { get; set; } = new();
    }
}