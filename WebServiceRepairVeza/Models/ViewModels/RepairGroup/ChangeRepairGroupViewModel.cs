using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebService.Models.ViewModels.RepairGroup
{
    public class ChangeRepairGroupViewModel
    {
        public int UserId { get; set; }
        [Display(Name = "Логин")]
        [NotNull]
        public string? UserName { get; set; }
        public List<RepairGroupViewModel> AllGroups { get; set; }
        public List<RepairGroupViewModel> UserGroups { get; set; }
        public ChangeRepairGroupViewModel()
        {
            AllGroups = new List<RepairGroupViewModel>();
            UserGroups = new List<RepairGroupViewModel>();
        }
    }
}
