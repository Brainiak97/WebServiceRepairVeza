using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    public class RepairLog : IEntityBase
    {
        public int Id { get; set; }
        [NotNull]
        public string? Malfunctions { get; set; }
        public RepairStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ChangedDate { get; set; }

        [InverseProperty("LogExecutors")]
        public ICollection<User>? Executors { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        [NotNull]
        public ICollection<RepairGroup>? RepairGroups { get; set; }

        public int AuthorId { get; set; }
        [InverseProperty("LogAuthors")]
        [NotNull]
        public User? Author { get; set; }
    }
}