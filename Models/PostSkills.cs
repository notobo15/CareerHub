using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class PostSkills
    {
        public int PostID { get; set; }


        public int SkillID { get; set; }


        [ForeignKey("SkillID")]
        public Skill Skill { get; set; }

        [ForeignKey("PostID")]
        public Post Post { get; set; }
    }
}
