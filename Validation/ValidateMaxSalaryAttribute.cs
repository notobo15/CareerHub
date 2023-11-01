using RecruitmentApp.Models;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Validation
{
    public class ValidateMaxSalaryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var post = (Post)validationContext.ObjectInstance;

            if ((double)value < post.MinSalary)
            {
                return new ValidationResult("MaxSalary must be greater than or equal to MinSalary.");
            }

            return ValidationResult.Success;
        }
    }
}
