using Core.Models;
using FluentValidation;

namespace BLL.Validator
{
    public class NotificationValidator : AbstractValidator<Notification>
    {
        public NotificationValidator()
        {
            RuleFor(item => item.Text).NotNull();
            RuleFor(item => item.Comment).NotNull();
            RuleFor(item => item.RecipientId).NotNull();
        }
    }
}
