using App.Application.Common.ValidationAttributes;
using MediatR;
using System.ComponentModel.DataAnnotations;
namespace App.Application.Auth.Command.Register
{
    public record RegisterCommand : IRequest<Guid>
    {
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;

        [UniqueEmail(errorMessage: "This Email Address has been used before.")]
        public string Email { get; set; } = null!;
        [CheckImageExtension(errorMessage: "Invalid image file format. Only .jpg, .png, and .jpeg files are allowed.")]
        public string? ImagePath { get; set; }
        public string? Phone { get; set; } = null!;
        public string? Address { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Confirmed Password field must match Password field")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
