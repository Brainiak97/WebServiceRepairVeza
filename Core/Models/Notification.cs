using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    public class Notification : IEntityBase
    {
        public int Id { get; set; }
        public bool IsRead { get; set; }
        [NotNull]
        public string? Text { get; set; }

        public int CommentId { get; set; }
        [NotNull]
        public Comment? Comment { get; set; }

        public int RecipientId { get; set; }
        [NotNull]
        public User? Recipient { get; set; }
    }
}
