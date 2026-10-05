using MediatR;
using System.ComponentModel.DataAnnotations;

namespace App.Application.Auth.Command.ChangePassword
{
    public record ChangePasswordCommand : IRequest<bool>
    {
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm Password must be equal to new password"), DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; } = null!;

    }

}
