using System.Diagnostics.CodeAnalysis;

namespace BLL.Models
{
    public class NotificationDto : IEntityBaseDto
    {
        public int Id { get; set; }
        public bool IsRead { get; set; }
        [NotNull]
        public string? Text { get; set; }

        public int CommentId { get; set; }
        [NotNull]
        public CommentDto? Comment { get; set; }

        public int RecipientId { get; set; }
        [NotNull]
        public UserDto? Recipient { get; set; }
    }
}
