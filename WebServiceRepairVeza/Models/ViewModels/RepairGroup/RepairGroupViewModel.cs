using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebService.Models.ViewModels.RepairLog;
using WebService.Models.ViewModels.User;

namespace WebService.Models.ViewModels.RepairGroup
{
    public class RepairGroupViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Краткое наименование службы")]
        [NotNull]
        public string? Name { get; set; }
        [Display(Name = "Полное наименование службы")]
        [NotNull]
        public string? Description { get; set; }

        public IEnumerable<UserViewModel>? Users { get; set; }
        public IEnumerable<RepairLogViewModel>? Logs { get; set; }
    }
}