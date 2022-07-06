using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebService.Models.ViewModels.User
{
    public class ChangePasswordViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Логин")]
        [NotNull]
        public string? UserName { get; set; }
        [Display(Name = "Старый пароль")]
        public string? OldPassword { get; set; }
        [Required]
        [Display(Name = "Новый пароль")]
        [NotNull]
        public string? NewPassword { get; set; }
    }
}