using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebService.Models.ViewModels.User
{
    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        [NotNull]
        public string? UserName { get; set; }
        [Required]
        [Display(Name = "Имя")]
        [NotNull]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        [NotNull]
        public string? SurName { get; set; }
        [Required]
        [Display(Name = "Отчество")]
        [NotNull]
        public string? MiddleName { get; set; }
        [Required]
        [Display(Name = "Номер телефона")]
        [NotNull]
        public string? PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Пароль")]
        [NotNull]
        public string? Password { get; set; }
    }
}