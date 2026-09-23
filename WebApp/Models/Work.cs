using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Work
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Client { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ContractAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CollectedAmount { get; set; }

        [Display(Name = "Estado")]
        public WorkStatus Status { get; set; } = WorkStatus.InProgress;


        [StringLength(500)]
        public string? Notes { get; set; }

        public ICollection<WorkWorker> WorkWorkers { get; set; } = new List<WorkWorker>();
    }
}
