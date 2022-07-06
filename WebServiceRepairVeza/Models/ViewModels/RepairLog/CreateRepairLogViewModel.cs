using WebService.Models.ViewModels.RepairGroup;

namespace WebService.Models.ViewModels.RepairLog
{
    public class CreateRepairLogViewModel
    {
        public List<RepairGroupViewModel> AllGroups { get; set; }
        public RepairLogViewModel RepairLog { get; set; }
        public CreateRepairLogViewModel()
        {
            AllGroups = new List<RepairGroupViewModel>();
            RepairLog = new RepairLogViewModel();
        }
    }
}
