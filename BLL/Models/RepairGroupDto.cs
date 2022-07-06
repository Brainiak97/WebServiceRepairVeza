using System.Diagnostics.CodeAnalysis;

namespace BLL.Models
{
    public class RepairGroupDto : IEntityBaseDto
    {
        public int Id { get; set; }
        [NotNull]
        public string? Name { get; set; }
        [NotNull]
        public string? Description { get; set; }

        public IEnumerable<UserDto>? Users { get; set; }
        public IEnumerable<RepairLogDto>? Logs { get; set; }
    }
}