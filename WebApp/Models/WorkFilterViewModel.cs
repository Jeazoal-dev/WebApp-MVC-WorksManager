namespace WebApp.Models
{
    public class WorkFilterViewModel
    {
        public string? Name { get; set; }
        public string? Client { get; set; }
        public WorkStatus? Status { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public decimal? ContractAmountMin { get; set; }
        public decimal? ContractAmountMax { get; set; }
        public List<Work> Results { get; set; } = new();
    }
}
