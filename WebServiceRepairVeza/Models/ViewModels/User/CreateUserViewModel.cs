using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebService.Models.ViewModels.User
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Недопустимая длина логина")]
        [RegularExpression(@"^[A-Za-zА-Яа-я0-9-.,_]+$", ErrorMessage = "Логин может содержать символы английского и русского алфавита, цирфы, а так же следующие символы - . , _")]
        [Display(Name = "Логин")]
        [NotNull]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Недопустимая длина имени")]
        [RegularExpression(@"^[А-Яа-я]+$", ErrorMessage = "Имя может содержать только символы русского алфавита")]
        [Display(Name = "Имя")]
        [NotNull]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Недопустимая длина фамилии")]
        [RegularExpression(@"^[А-Яа-я]+$", ErrorMessage = "Имя может содержать только символы русского алфавита")]
        [Display(Name = "Фамилия")]
        [NotNull]
        public string? SurName { get; set; }

        [Required(ErrorMessage = "Введите отчество")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Недопустимая длина отчества")]
        [RegularExpression(@"^[А-Яа-я]+$", ErrorMessage = "Имя может содержать только символы русского алфавита")]
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
    }
}