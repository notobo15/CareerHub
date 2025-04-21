using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class CompanyType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyTypeId { get; set; }
        [Required]
        [Display(Name = "Tên loại công ty")]
        public string Name { get; set; }
        public string Slug { get; set; }

        public ICollection<Company> Companies { get; set; }
    }
}
