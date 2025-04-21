using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.ModelViews
{
    public class ResumeUploadViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn file.")]
        [Display(Name = "Chọn CV")]
        public IFormFile CvFile { get; set; }

        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
