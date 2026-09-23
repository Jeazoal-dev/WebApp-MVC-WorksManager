using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class PaymentFilterViewModel
    {
        [Display(Name = "Obra")]
        public int? WorkId { get; set; }

        [Display(Name = "Trabajador")]
        public int? WorkerId { get; set; }

        [Display(Name = "Método de pago")]
        public string? PaymentMethod { get; set; }

        [Display(Name = "Banco")]
        public string? Bank { get; set; }

        [Display(Name = "Referencia")]
        public string? Reference { get; set; }

        [Display(Name = "Fecha desde")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Fecha hasta")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Monto mín.")]
        public decimal? AmountMin { get; set; }

        [Display(Name = "Monto máx.")]
        public decimal? AmountMax { get; set; }

        [Display(Name = "Estado")]
        public bool? Cancelled { get; set; }

        // Dropdowns
        public List<Work> Works { get; set; } = new();
        public List<Worker> Workers { get; set; } = new();

        // Resultados
        public List<Payment> Results { get; set; } = new();
    }
}