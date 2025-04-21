using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.ModelViews
{
    public class AboutMeViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập phần giới thiệu bản thân.")]
        [MaxLength(2500, ErrorMessage = "Tối đa 2500 ký tự.")]
        [Display(Name = "About Me")]
        public string AboutMe { get; set; }
    }
}
