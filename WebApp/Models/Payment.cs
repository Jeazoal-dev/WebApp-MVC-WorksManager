using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int WorkWorkerId { get; set; }

        public WorkWorker? WorkWorker { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(50)]
        public string? Bank { get; set; }

        [StringLength(30)]
        public string? Reference { get; set; }

        public string? UserId { get; set; }

        public bool Cancelled { get; set; } = false;

        [StringLength(300)]
        public string? Notes { get; set; }
    }
}