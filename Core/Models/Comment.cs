using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    public class Comment : IEntityBase
    {
        public int Id { get; set; }
        [NotNull]
        public string? Text { get; set; }
        public DateTime Date { get; set; }

        public int RepairLogId { get; set; }
        [NotNull]
        public RepairLog? RepairLog { get; set; }

        public int CommentatorId { get; set; }
        [NotNull]
        public User? Commentator { get; set; }

        [NotNull]
        public ICollection<Notification>? Notifications { get; set; }
    }
}
