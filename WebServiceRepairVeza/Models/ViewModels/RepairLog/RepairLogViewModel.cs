using Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebService.Models.ViewModels.Comment;
using WebService.Models.ViewModels.RepairGroup;
using WebService.Models.ViewModels.User;

namespace WebService.Models.ViewModels.RepairLog
{
    public class RepairLogViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Выявленные неисправности")]
        [NotNull]
        [Required(ErrorMessage = "Не указаны неисправности")]
        public string? Malfunctions { get; set; }
        [Display(Name = "Статус заявки")]
        public RepairStatus Status { get; set; }
        [Display(Name = "Дата создания заявки")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Изменено")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ChangedDate { get; set; }

        [Display(Name = "Исполнители")]
        public IEnumerable<UserViewModel>? Executors { get; set; }
        [Display(Name = "Комментарии")]
        public ICollection<CommentViewModel>? Comments { get; set; }
        [Display(Name = "Ремонтные группы")]
        [NotNull]
        public IEnumerable<RepairGroupViewModel>? RepairGroups { get; set; }

        public int AuthorId { get; set; }
        [Display(Name = "Автор")]
        [NotNull]
        public UserViewModel? Author { get; set; }
    }
}