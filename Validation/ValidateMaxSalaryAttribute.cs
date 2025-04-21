using RecruitmentApp.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Validation
{
    public class ValidateMaxSalaryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();

            var minProp = type.GetProperty("MinSalary");
            var maxProp = type.GetProperty("MaxSalary");

            if (minProp == null || maxProp == null)
                return ValidationResult.Success;

            double minSalary = Convert.ToDouble(minProp.GetValue(instance));
            double maxSalary = Convert.ToDouble(maxProp.GetValue(instance));

            if (minSalary > maxSalary)
            {
                return new ValidationResult("Lương tối thiểu không được lớn hơn lương tối đa.");
            }

            return ValidationResult.Success;
        }
    }
}
