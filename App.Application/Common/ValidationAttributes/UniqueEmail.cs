using App.Application.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace App.Application.Common.ValidationAttributes
{
    public class UniqueEmail : ValidationAttribute
    {
        private readonly string _errorMessage;
        public UniqueEmail(string errorMessage) : base(errorMessage)
        {
            _errorMessage = errorMessage;

        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? email = value as string;
            var userService = validationContext.GetService<IIdentityService>();
            bool emailFound = userService!.CheckEmailUniqueness(email!);
            if (!emailFound)
                return ValidationResult.Success;
            return new ValidationResult(errorMessage: _errorMessage);

        }
    }
}
