using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Application.Common.ValidationAttributes
{
    public class CheckImageSize : ValidationAttribute
    {
        private readonly int _maxSizeInBytes;
        private readonly string _errorMessage;
        public CheckImageSize(string errorMessage, int maxSizeInMB = 5)
        {
            _errorMessage = errorMessage;
            _maxSizeInBytes = maxSizeInMB * 1024 * 1024;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file && file.Length > _maxSizeInBytes)
                return new ValidationResult(_errorMessage);
            return ValidationResult.Success;

        }
    }
}
