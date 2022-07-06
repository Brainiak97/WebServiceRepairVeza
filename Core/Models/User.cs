using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    public class User : IdentityUser<int>, IEntityBase
    {
        [Required]
        [NotNull]
        public string? Name { get; set; }
        [Required]
        [NotNull]
        public string? SurName { get; set; }
        [Required]
        [NotNull]
        public string? MiddleName { get; set; }

        public IEnumerable<Comment>? Comments { get; set; }
        public IEnumerable<RepairLog>? LogExecutors { get; set; }
        public IEnumerable<RepairLog>? LogAuthors { get; set; }
        public ICollection<RepairGroup>? RepairGroups { get; set; }
        public IEnumerable<Notification>? Notifications { get; set; }
    }
}