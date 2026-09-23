using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class WorkFilterViewModel
    {
        [Display(Name = "Nombre")]
        public string? Name { get; set; }

        [Display(Name = "Cliente")]
        public string? Client { get; set; }

        [Display(Name = "Estado")]
        public WorkStatus? Status { get; set; }

        [Display(Name = "Inicio desde")]
        [DataType(DataType.Date)]
        public DateTime? StartDateFrom { get; set; }

        [Display(Name = "Inicio hasta")]
        [DataType(DataType.Date)]
        public DateTime? StartDateTo { get; set; }

        [Display(Name = "Fin desde")]
        [DataType(DataType.Date)]
        public DateTime? EndDateFrom { get; set; }

        [Display(Name = "Fin hasta")]
        [DataType(DataType.Date)]
        public DateTime? EndDateTo { get; set; }

        [Display(Name = "Monto mín.")]
        public decimal? ContractAmountMin { get; set; }

        [Display(Name = "Monto máx.")]
        public decimal? ContractAmountMax { get; set; }

        // Resultados
        public List<Work> Results { get; set; } = new();
    }
}