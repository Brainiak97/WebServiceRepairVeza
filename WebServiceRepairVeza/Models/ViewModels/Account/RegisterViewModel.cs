using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebService.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Недопустимая длина имени")]
        [Display(Name = "Логин")]
        [NotNull]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Недопустимая длина имени")]
        [Display(Name = "Имя")]
        [NotNull]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Введите фамилия")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Недопустимая длина имени")]
        [Display(Name = "Фамилия")]
        [NotNull]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Введите отчество")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Недопустимая длина имени")]
        [Display(Name = "Отчество")]
        [NotNull]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Введите номер телефона")]
        [RegularExpression(@"^\+375\(\d{2}\) \d{3}-\d{2}-\d{2}$", ErrorMessage = "Неверный формат номера телефона")]
        [Display(Name = "Телефон")]
        [NotNull]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password, ErrorMessage = "Пароль должен состоять минимум из 4 символов")]
        [Display(Name = "Пароль")]
        [NotNull]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Введите пароль повторно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        [NotNull]
        public string? PasswordConfirm { get; set; }
    }
}
