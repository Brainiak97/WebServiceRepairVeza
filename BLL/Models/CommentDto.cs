using System.Diagnostics.CodeAnalysis;

namespace BLL.Models
{
    public class CommentDto : IEntityBaseDto
    {
        public int Id { get; set; }
        [NotNull]
        public string? Text { get; set; }
        public DateTime Date { get; set; }

        public int RepairLogId { get; set; }
        [NotNull]
        public RepairLogDto? RepairLog { get; set; }

        public int CommentatorId { get; set; }
        [NotNull]
        public UserDto? Commentator { get; set; }

        public IEnumerable<NotificationDto>? Notifications { get; set; }
    }
}
