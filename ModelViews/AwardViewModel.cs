using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.ModelViews
{
    public class AwardViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên giải thưởng.")]
        [Display(Name = "Awards name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tổ chức.")]
        [Display(Name = "Award organization")]
        public string Organization { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tháng.")]
        [Range(1, 12, ErrorMessage = "Tháng không hợp lệ.")]
        [Display(Name = "Issue Month")]
        public int IssueMonth { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn năm.")]
        [Range(1950, 2100, ErrorMessage = "Năm không hợp lệ.")]
        [Display(Name = "Issue Year")]
        public int IssueYear { get; set; }

        [MaxLength(1000, ErrorMessage = "Mô tả không vượt quá 1000 ký tự.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
