using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class WorkWorker
    {
        public int Id { get; set; }

        [Required]
        public int WorkId { get; set; }
        public Work Work { get; set; } = null!;

        [Required]
        public int WorkerId { get; set; }
        public Worker Worker { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AgreedAmount { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
