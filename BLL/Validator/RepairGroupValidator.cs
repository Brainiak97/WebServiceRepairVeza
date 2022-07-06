using Core.Models;
using FluentValidation;

namespace BLL.Validator
{
    public class RepairGroupValidator : AbstractValidator<RepairGroup>
    {
        public RepairGroupValidator()
        {
            RuleFor(item => item.Name).NotNull();
            RuleFor(item => item.Description).NotNull();
        }
    }
}
