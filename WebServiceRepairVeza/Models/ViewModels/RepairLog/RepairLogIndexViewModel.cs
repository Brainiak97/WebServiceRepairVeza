namespace WebService.Models.ViewModels.RepairLog
{
    public class RepairLogIndexViewModel
    {
        public IEnumerable<RepairLogViewModel>? RepairLogs { get; set; }
        public PageViewModel? PageViewModel { get; set; }
    }
}
