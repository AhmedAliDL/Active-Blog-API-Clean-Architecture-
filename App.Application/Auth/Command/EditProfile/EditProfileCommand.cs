using App.Application.Common.ValidationAttributes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace App.Application.Auth.Command.EditProfile
{
    public record EditProfileCommand : IRequest<IdentityResult>
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [UniqueEmail(errorMessage: "This Email Address has been used before.")]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirmed Password field must match New Password field")]
        public string? ConfirmNewPassword { get; set; }
        [CheckImageExtension(errorMessage: "Invalid image file format. Only .jpg, .png, and .jpeg files are allowed.")]
        public string? ImagePath { get; set; }
    }
}
