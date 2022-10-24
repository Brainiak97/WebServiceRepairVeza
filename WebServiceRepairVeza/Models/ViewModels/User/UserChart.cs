using System.Diagnostics.CodeAnalysis;

namespace WebService.Models.ViewModels.User
{
    public class UserChart
    {
        public UserViewModel User { get; set; }
        public Chart ChartDoughnut { get; set; }
        public Chart ChartBar { get; set; }

        public UserChart(UserViewModel _user, Chart _chartDoughnut, Chart _chartBar)
        {
            User = _user;
            ChartDoughnut = _chartDoughnut;
            ChartBar = _chartBar;
        }
    }
}
