using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Attributes
{
    public class RequiredIfNotStudyingAttribute : ValidationAttribute
    {
        private readonly string _boolPropertyName;

        public RequiredIfNotStudyingAttribute(string boolPropertyName)
        {
            _boolPropertyName = boolPropertyName;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var isStudyingProp = validationContext.ObjectType.GetProperty(_boolPropertyName);
            if (isStudyingProp == null)
                return new ValidationResult($"Unknown property: {_boolPropertyName}");

            var isStudying = (bool)isStudyingProp.GetValue(validationContext.ObjectInstance)!;

            // Nếu không đang học thì field bắt buộc
            if (!isStudying && (value == null || string.IsNullOrEmpty(value.ToString())))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }

}
