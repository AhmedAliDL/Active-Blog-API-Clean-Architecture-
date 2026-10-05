using MediatR;
using System.ComponentModel.DataAnnotations;

namespace App.Application.Auth.Command.ResetPassword
{
    public record ResetPasswordCommand : IRequest<bool>
    {
        public string ConfirmationToken { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm Password must be equal to new password"), DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; } = string.Empty;

    }
}
