using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RecruitmentApp.ModelViews
{
    public class PersonalDetailsViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chức danh.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        [Display(Name = "Gender")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố.")]
        [Display(Name = "Current province/city")]
        public string Province { get; set; }

        [Display(Name = "Address (Street, district,...)")]
        public string Address { get; set; }

        [Url(ErrorMessage = "Link không hợp lệ.")]
        [Display(Name = "Personal link (Linkedin, porfolio,...)")]
        public string PersonalLink { get; set; }

        public string AvatarUrl { get; set; }
        public IFormFile AvatarFile { get; set; }
    }

}
