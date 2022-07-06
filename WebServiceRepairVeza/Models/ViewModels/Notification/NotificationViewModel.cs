using System.Diagnostics.CodeAnalysis;
using WebService.Models.ViewModels.Comment;
using WebService.Models.ViewModels.User;

namespace WebService.Models.ViewModels.Notification
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public bool IsRead { get; set; }
        [NotNull]
        public string? Text { get; set; }

        public int CommentId { get; set; }
        [NotNull]
        public CommentViewModel? Comment { get; set; }

        public int? RecipientId { get; set; }
        public UserViewModel? Recipient { get; set; }
    }
}
