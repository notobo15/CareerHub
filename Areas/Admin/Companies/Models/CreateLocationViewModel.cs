using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecruitmentApp.Areas.Admin.Companies.Models
{
    public class CreateLocationViewModel
    {
        public int CompanyId { get; set; }

        [Required]
        public string ProvinceCode { get; set; }

        [Required]
        public string DistrictCode { get; set; }

        [Required]
        public string WardCode { get; set; }

        public string DetailPosition { get; set; }

        // Dữ liệu dropdown
        public List<SelectListItem> Provinces { get; set; }
        public List<SelectListItem> Districts { get; set; }
        public List<SelectListItem> Wards { get; set; }
    }

}
