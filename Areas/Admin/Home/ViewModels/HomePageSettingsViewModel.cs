using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Areas.Admin.Home.ViewModels
{
    public class HomePageSettingsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng bài viết.")]
        [Range(1, 100, ErrorMessage = "Số lượng bài viết phải từ 1 đến 100.")]
        public int NumberOfPosts { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng công ty.")]
        [Range(1, 100, ErrorMessage = "Số lượng công ty phải từ 1 đến 100.")]
        public int NumberOfCompanies { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã số thuế.")]
        [StringLength(20, ErrorMessage = "Mã số thuế không được dài quá 20 ký tự.")]
        public string TaxNumber { get; set; }
    }
}
