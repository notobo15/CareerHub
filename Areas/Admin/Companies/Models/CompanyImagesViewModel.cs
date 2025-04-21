using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RecruitmentApp.Areas.Admin.Companies.Models
{
    public class CompanyImagesViewModel
    {
        [Required]
        public int CompanyId { get; set; }

        public IFormFileCollection NewImages { get; set; }

        public string DeletedImageIds { get; set; }

        public string ImageOrder { get; set; }
    }
}
