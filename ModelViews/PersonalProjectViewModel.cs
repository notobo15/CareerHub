using System.ComponentModel.DataAnnotations;
using RecruitmentApp.Attributes;

namespace RecruitmentApp.ModelViews
{
    public class PersonalProjectViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dự án.")]
        [StringLength(255)]
        public string ProjectName { get; set; }

        public bool IsOngoing { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tháng bắt đầu.")]
        [Range(1, 12)]
        public int StartMonth { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn năm bắt đầu.")]
        [Range(1950, 2100)]
        public int StartYear { get; set; }

        [Range(1, 12)]
        [RequiredIfNotStudying("IsOngoing", ErrorMessage = "Vui lòng chọn tháng kết thúc.")]
        public int? EndMonth { get; set; }

        [Range(1950, 2100)]
        [RequiredIfNotStudying("IsOngoing", ErrorMessage = "Vui lòng chọn năm kết thúc.")]
        public int? EndYear { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Đường dẫn không hợp lệ.")]
        public string? UrlProject { get; set; }
    }
}
