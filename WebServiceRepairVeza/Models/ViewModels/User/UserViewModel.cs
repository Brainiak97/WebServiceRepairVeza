using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebService.Models.ViewModels.Comment;
using WebService.Models.ViewModels.Notification;
using WebService.Models.ViewModels.RepairGroup;
using WebService.Models.ViewModels.RepairLog;

namespace WebService.Models.ViewModels.User
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Логин")]
        [NotNull]
        public string? UserName { get; set; }
        [Required]
        [Display(Name = "Имя")]
        [NotNull]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        [NotNull]
        public string? SurName { get; set; }
        [Required]
        [Display(Name = "Отчество")]
        [NotNull]
        public string? MiddleName { get; set; }
        [Required]
        [Display(Name = "Номер телефона")]
        [NotNull]
        public string? PhoneNumber { get; set; }

        public IEnumerable<RepairGroupViewModel>? RepairGroups { get; set; }
        public IEnumerable<CommentViewModel>? Comments { get; set; }
        public IEnumerable<RepairLogViewModel>? LogAuthors { get; set; }
        public IEnumerable<RepairLogViewModel>? LogExecutors { get; set; }
        public IEnumerable<NotificationViewModel>? Notifications { get; set; }
    }
}