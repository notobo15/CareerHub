using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class CompanySkills
    {
        public int CompanyID { get; set; }

       
        public int SkillID { get; set; }


        [ForeignKey("SkillID")]
        public Skill Skill { get; set; }

        [ForeignKey("CompanyID")]
        public Company Company { get; set; }

    }
}
