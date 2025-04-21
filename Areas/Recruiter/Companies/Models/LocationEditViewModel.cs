using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Areas.Recruiter.Companies.Models
{
    public class LocationEditViewModel
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
        public string ProvinceCode { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public string DistrictCode { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public string WardCode { get; set; }

        public string DetailPosition { get; set; }
    }
}
