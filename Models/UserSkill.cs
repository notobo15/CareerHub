using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class UserSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required]
        public int SkillId { get; set; }

        [ForeignKey("SkillId")]
        public Skill Skill { get; set; }

        public string SkillLevel { get; set; }
    }
}
