using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace WebService.Models.ViewModels.User
{
    public class Chart
    {
        [NotNull]
        public string[]? Lables { get; set; }
        [NotNull]
        public double[]? Data { get; set; }
        [NotNull]
        public string[]? Backgrounds { get; set; }
    }
}
