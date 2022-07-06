using WebService.Models.ViewModels.User;

namespace WebService.Models.ViewModels.RepairLog
{
    public class ChangeRepairLogExecutorsViewModel
    {
        public int LogId { get; set; }
        public List<UserViewModel> LogExecutors { get; set; }
        public List<UserViewModel> AllExecutors { get; set; }
        public ChangeRepairLogExecutorsViewModel()
        {
            LogExecutors = new List<UserViewModel>();
            AllExecutors = new List<UserViewModel>();
        }
    }
}
