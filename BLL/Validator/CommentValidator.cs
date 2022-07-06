using Core.Models;
using FluentValidation;

namespace BLL.Validator
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(item => item.RepairLogId).NotNull();
            RuleFor(item => item.CommentatorId).NotNull();
            RuleFor(item => item.Date).NotNull();
            RuleFor(item => item.Text).NotNull();
        }
    }
}
