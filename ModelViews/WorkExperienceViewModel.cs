using System.ComponentModel.DataAnnotations;
using RecruitmentApp.Attributes;
using RecruitmentApp.Validation;

namespace RecruitmentApp.ModelViews
{
    [WorkExperienceDateValidation(ErrorMessage = "Có lỗi trong thời gian làm việc.")]
    public class WorkExperienceViewModel
    {
        public int Id { get; set; }

         [Required(ErrorMessage = "Vui lòng nhập chức danh công việc.")]
        [StringLength(255)]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên công ty.")]
        [StringLength(255)]
        public string Company { get; set; }

        public bool IsCurrentlyWorking { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tháng bắt đầu.")]
        [Range(1, 12, ErrorMessage = "Tháng bắt đầu không hợp lệ.")]
        public int FromMonth { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn năm bắt đầu.")]
        [Range(1950, 2100, ErrorMessage = "Năm bắt đầu không hợp lệ.")]
        public int FromYear { get; set; }

        [Range(1, 12)]
        [RequiredIfNotStudying("IsCurrentlyWorking", ErrorMessage = "Vui lòng chọn tháng kết thúc.")]
        public int? ToMonth { get; set; }

        [Range(1950, 2100)]
        [RequiredIfNotStudying("IsCurrentlyWorking", ErrorMessage = "Vui lòng chọn năm kết thúc.")]
        public int? ToYear { get; set; }

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự.")]
        public string? Description { get; set; }

        [StringLength(1000, ErrorMessage = "Thông tin dự án không được vượt quá 1000 ký tự.")]
        public string? Project { get; set; }
    }
}
