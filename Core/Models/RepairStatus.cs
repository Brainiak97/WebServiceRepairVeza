using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public enum RepairStatus
    {
        [Display(Name = "Запрос")]
        Request,
        [Display(Name = "В работе")]
        AtWork,
        [Display(Name = "Проверка")]
        Check,
        [Display(Name = "Готово")]
        Completed,
        [Display(Name = "Архив")]
        Archive
    }
}