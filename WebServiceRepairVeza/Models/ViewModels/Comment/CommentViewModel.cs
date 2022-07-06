using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebService.Models.ViewModels.Notification;
using WebService.Models.ViewModels.RepairLog;
using WebService.Models.ViewModels.User;

namespace WebService.Models.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Текст")]
        [NotNull]
        public string? Text { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        public int RepairLogId { get; set; }
        [NotNull]
        public RepairLogViewModel? RepairLog { get; set; }

        public int CommentatorId { get; set; }
        public UserViewModel? Commentator { get; set; }

        public IEnumerable<NotificationViewModel>? Notifications { get; set; }
    }
}
