using System.ComponentModel.DataAnnotations;
using System;

namespace RecruitmentApp.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WorkExperienceDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = value as ModelViews.WorkExperienceViewModel;

            if (model == null)
                return ValidationResult.Success;

            // Kiểm tra nếu không đang làm việc thì ToMonth & ToYear bắt buộc
            if (!model.IsCurrentlyWorking)
            {
                if (!model.ToMonth.HasValue)
                {
                    return new ValidationResult("Vui lòng chọn tháng kết thúc.");
                }

                if (!model.ToYear.HasValue)
                {
                    return new ValidationResult("Vui lòng chọn năm kết thúc.");
                }
            }

            // So sánh From < To nếu có đủ dữ liệu
            if (model.ToMonth.HasValue && model.ToYear.HasValue)
            {
                var fromDate = new DateTime(model.FromYear, model.FromMonth, 1);
                var toDate = new DateTime(model.ToYear.Value, model.ToMonth.Value, 1);

                if (fromDate >= toDate)
                {
                    return new ValidationResult("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
