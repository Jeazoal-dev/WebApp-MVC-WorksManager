using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class WorkerFilterViewModel
    {
        [Display(Name = "Nombre")]
        public string? Name { get; set; }

        [Display(Name = "Documento")]
        public string? Document { get; set; }

        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Display(Name = "Banco")]
        public string? Bank { get; set; }

        [Display(Name = "Estado")]
        public string? Status { get; set; }

        // Paginación
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public string? SortBy { get; set; }
        public string SortDir { get; set; } = "asc";
        public List<Worker> Results { get; set; } = new();
    }
}