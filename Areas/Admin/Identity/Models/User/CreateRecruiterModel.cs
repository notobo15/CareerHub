using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecruitmentApp.Areas.Admin.Identity.Models.User
{
    public class CreateRecruiterModel
    {
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Recruiter";

        [Display(Name = "Công ty")]
        public int CompanyId { get; set; }

        // Dropdown list support
        public List<SelectListItem>? Companies { get; set; }
    }
}
