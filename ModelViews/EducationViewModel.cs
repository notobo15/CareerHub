using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using RecruitmentApp.Attributes;
using RecruitmentApp.Validation;

namespace RecruitmentApp.ModelViews
{
    public class EducationViewModel : IValidatableObject
    {
        public int EducationId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên trường.")]
        public string School { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chuyên ngành.")]
        public string Major { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tháng bắt đầu.")]
        [Range(1, 12)]
        public int FromMonth { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn năm bắt đầu.")]
        [Range(1950, 2100)]
        public int FromYear { get; set; }

        [Range(1, 12)]
        [RequiredIfNotStudying("IsStudying", ErrorMessage = "Vui lòng chọn tháng kết thúc.")]
        public int? ToMonth { get; set; }

        [Range(1950, 2100)]
        [RequiredIfNotStudying("IsStudying", ErrorMessage = "Vui lòng chọn năm kết thúc.")]
        public int? ToYear { get; set; }

        public bool IsStudying { get; set; }

        [MaxLength(1000)]
        public string? Details { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsStudying && ToYear.HasValue && ToMonth.HasValue)
            {
                var fromDate = new DateTime(FromYear, FromMonth, 1);
                var toDate = new DateTime(ToYear.Value, ToMonth.Value, 1);

                if (fromDate >= toDate)
                {
                    yield return new ValidationResult(
                        "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.",
                        new[] { "" } // <- để lỗi hiển thị ở ValidationSummary, không liên kết với trường cụ thể
                    );
                }
            }
        }
    }
}
