using Core.Models;
using FluentValidation;

namespace BLL.Validator
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(item => item.Id).NotEmpty();
            RuleFor(item => item.UserName).NotNull();
            RuleFor(item => item.Name).NotNull();
            RuleFor(item => item.SurName).NotNull();
            RuleFor(item => item.MiddleName).NotNull();
            RuleFor(item => item.PhoneNumber).NotNull();
        }
    }
}
