using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RecruitmentApp.Models
{
    public class Industry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IndustryId { get; set; }

        public string Name { get; set; }

        public ICollection<CompanyIndustry> CompanyIndustries { get; set; }

    }
}
