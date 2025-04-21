using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.ModelViews
{
    public class CertificateViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên chứng chỉ.")]
        [Display(Name = "Certificate Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tổ chức.")]
        [Display(Name = "Organization")]
        public string Organization { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tháng.")]
        [Range(1, 12, ErrorMessage = "Tháng không hợp lệ.")]
        [Display(Name = "Issue Month")]
        public int IssueMonth { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn năm.")]
        [Range(1950, 2100, ErrorMessage = "Năm không hợp lệ.")]
        [Display(Name = "Issue Year")]
        public int IssueYear { get; set; }

        [Url(ErrorMessage = "Liên kết không hợp lệ.")]
        [Display(Name = "Certificate URL")]
        public string? Url { get; set; }

        [MaxLength(1000, ErrorMessage = "Mô tả không vượt quá 1000 ký tự.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
