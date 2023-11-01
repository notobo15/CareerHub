namespace RecruitmentApp.Validation
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;

    public class ValidateImageFileAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file == null)
            {
                //return new ValidationResult("Please select a file.");
                return ValidationResult.Success;
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult("Only image files with extensions .jpg, .jpeg, .png, and .gif are allowed.");
            }

            return ValidationResult.Success;
        }
    }
}
