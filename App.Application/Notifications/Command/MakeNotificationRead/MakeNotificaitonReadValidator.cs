using FluentValidation;

namespace App.Application.Notifications.Command.MakeNotificationRead
{
    public class MakeNotificaitonReadValidator : AbstractValidator<MakeNotificationReadCommand>
    {
        public MakeNotificaitonReadValidator()
        {
            RuleFor(n => n.NotificationId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Notification id is required.");
        }
    }
}
