using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public enum WorkStatus
    {
        [Display(Name = "En progreso")]
        InProgress = 1,

        [Display(Name = "Pausado")]
        Paused = 2,

        [Display(Name = "Finalizado")]
        Finished = 3,

        [Display(Name = "Cancelado")]
        Cancelled = 4
    }
}