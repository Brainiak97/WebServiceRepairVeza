using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    public class RepairGroup : IEntityBase
    {
        public int Id { get; set; }
        [NotNull]
        public string? Name { get; set; }
        [NotNull]
        public string? Description { get; set; }

        public IEnumerable<User>? Users { get; set; }
        public IEnumerable<RepairLog>? Logs { get; set; }
    }
}