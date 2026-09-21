using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Worker
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Document { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public bool Active { get; set; } = true;

        [StringLength(50)]
        public string? Bank { get; set; }

        [StringLength(30)]
        public string? AccountNumber { get; set; }

        public ICollection<WorkWorker> WorkWorkers { get; set; } = new List<WorkWorker>();
    }
}
