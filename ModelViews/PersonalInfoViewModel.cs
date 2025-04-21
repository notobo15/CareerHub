using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecruitmentApp.ModelViews
{
    public class PersonalInfoViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nơi làm việc mong muốn.")]
        [Display(Name = "Preferred work location")]
        [MaxLength(3, ErrorMessage = "Chỉ được chọn tối đa 3 địa điểm.")]
        public List<string> PreferredLocations { get; set; } = new List<string>();

        public MultiSelectList Provinces { get; set; }
    }
}
