using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class CompanySkills
    {

        public int SkillID { get; set; }
        [ForeignKey("SkillID")]
        public Skill Skill { get; set; }

        public int CompanyID { get; set; }
        [ForeignKey("CompanyID")]
        public Company Company { get; set; }

    }
}
