using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Areas.User.Jobs.ModelViews
{
    public class ApplyPostViewModel
    {
        public int PostId { get; set; }
        public string slugPost { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string Phone { get; set; }

        public string Description { get; set; } // Cover letter

        public IFormFile UploadedFile { get; set; } // File mới nếu user không dùng CV mặc định

        // CV mặc định
        public int? PrimaryResumeId { get; set; } // ResumeFileId nếu có

        public string PrimaryResumeName { get; set; } // Tên gốc của file CV
        public string PrimaryResumePath { get; set; } // Đường dẫn CV
    }
}
