namespace WebApp.Models
{
    public class WorkerEditViewModel
    {
        public Worker Worker { get; set; } = null!;

        public int TotalWorks { get; set; }
        public decimal TotalAgreed { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }

        public List<WorkWorker> Assignments { get; set; } = new List<WorkWorker>();
    }
}