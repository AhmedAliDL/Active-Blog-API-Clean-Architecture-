using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Application.Common.ValidationAttributes
{
    public class CheckImageExtension : ValidationAttribute
    {
        private readonly string _errorMessage;
        public CheckImageExtension(string errorMessage) : base(errorMessage)
        {
            _errorMessage = errorMessage;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new ValidationResult(_errorMessage);
                }
            }
            return ValidationResult.Success;
        }

    }
}
