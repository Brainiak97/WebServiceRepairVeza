using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebService.Models.ViewModels.User
{
    public class ChangePasswordViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Логин")]
        [NotNull]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Введите старый пароль")]
        [DataType(DataType.Password, ErrorMessage = "Пароль должен состоять минимум из 4 символов")]
        [Display(Name = "Старый пароль")]
        [NotNull]
        public string? OldPassword { get; set; }
        [Required(ErrorMessage = "Введите новый пароль")]
        [DataType(DataType.Password, ErrorMessage = "Пароль должен состоять минимум из 4 символов")]
        [Display(Name = "Новый пароль")]
        [NotNull]
        public string? NewPassword { get; set; }
    }
}