using System.Diagnostics.CodeAnalysis;

namespace BLL.Models
{
    public class UserDto : IEntityBaseDto
    {
        public int Id { get; set; }
        [NotNull]
        public string? Name { get; set; }
        [NotNull]
        public string? SurName { get; set; }
        [NotNull]
        public string? MiddleName { get; set; }
        [NotNull]
        public string? PhoneNumber { get; set; }
        [NotNull]
        public string? UserName { get; set; }

        public IEnumerable<CommentDto>? Comments { get; set; }
        public IEnumerable<RepairLogDto>? LogExecutors { get; set; }
        public IEnumerable<RepairLogDto>? LogAuthors { get; set; }
        public IEnumerable<RepairGroupDto>? RepairGroups { get; set; }
        public IEnumerable<NotificationDto>? Notifications { get; set; }
    }
}
