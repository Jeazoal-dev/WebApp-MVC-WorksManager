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

        public List<Worker> Results { get; set; } = new();
    }
}