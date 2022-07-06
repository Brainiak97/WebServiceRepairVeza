using Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace BLL.Models
{
    public class RepairLogDto : IEntityBaseDto
    {
        public int Id { get; set; }
        [NotNull]
        public string? Malfunctions { get; set; }
        public RepairStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ChangedDate { get; set; }

        public ICollection<UserDto>? Executors { get; set; }
        public ICollection<CommentDto>? Comments { get; set; }
        [NotNull]
        public ICollection<RepairGroupDto>? RepairGroups { get; set; }

        public int AuthorId { get; set; }
        [NotNull]
        public UserDto? Author { get; set; }
    }
}