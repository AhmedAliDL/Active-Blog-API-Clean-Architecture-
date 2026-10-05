using FluentValidation;

namespace App.Application.Notifications.Command.NotifyAdminMail
{
    public class NotifyAdminMailValidator : AbstractValidator<NotifyAdminMailCommand>
    {
        public NotifyAdminMailValidator()
        {
            RuleFor(sm => sm.Title)
                .MinimumLength(3)
                .WithMessage("Subject must be at least with 3 character.")
                .MaximumLength(20)
                .WithMessage("Subject must be at most with 20 character.");
            RuleFor(sm => sm.Message)
                .MinimumLength(3)
                .WithMessage("Body must be at least with 3 character.")
                .MaximumLength(300)
                .WithMessage("Body must be at most with 300 character.");
        }
    }
}
