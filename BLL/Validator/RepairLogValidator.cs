using BLL.Models;
using Core.Models;
using FluentValidation;

namespace BLL.Validator
{
    public class RepairLogValidator : AbstractValidator<RepairLog>
    {
        public RepairLogValidator()
        {
            RuleFor(item => item.Malfunctions).NotNull();
            RuleFor(item => item.RequestDate).NotNull();
            RuleFor(item => item.RepairGroups).NotNull();
            RuleFor(item => item.Status).NotNull();
            RuleFor(item => item.AuthorId).NotNull();
        }
    }
}
