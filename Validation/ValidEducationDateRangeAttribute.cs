using System.ComponentModel.DataAnnotations;
using System;

namespace RecruitmentApp.Validation
{
    public class ValidEducationDateRangeAttribute : ValidationAttribute
    {
        private readonly string _fromMonthProperty;
        private readonly string _fromYearProperty;
        private readonly string _isStudyingProperty;

        public ValidEducationDateRangeAttribute(string fromMonthProperty, string fromYearProperty, string isStudyingProperty)
        {
            _fromMonthProperty = fromMonthProperty;
            _fromYearProperty = fromYearProperty;
            _isStudyingProperty = isStudyingProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var toMonthProp = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var toMonth = (int?)toMonthProp?.GetValue(validationContext.ObjectInstance);

            var toYearProp = validationContext.ObjectType.GetProperty("ToYear");
            var toYear = (int?)toYearProp?.GetValue(validationContext.ObjectInstance);

            var fromMonth = (int)validationContext.ObjectType.GetProperty(_fromMonthProperty)?.GetValue(validationContext.ObjectInstance);
            var fromYear = (int)validationContext.ObjectType.GetProperty(_fromYearProperty)?.GetValue(validationContext.ObjectInstance);
            var isStudying = (bool)validationContext.ObjectType.GetProperty(_isStudyingProperty)?.GetValue(validationContext.ObjectInstance);

            if (isStudying) return ValidationResult.Success;

            if (!toMonth.HasValue || !toYear.HasValue)
                return ValidationResult.Success; // được kiểm tra bởi [RequiredIfNotStudying]

            var fromDate = new DateTime(fromYear, fromMonth, 1);
            var toDate = new DateTime(toYear.Value, toMonth.Value, 1);

            if (toDate < fromDate)
            {
                return new ValidationResult(ErrorMessage, new[] { "ToYear" });
            }

            return ValidationResult.Success!;
        }
    }
}
