using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }
        [Required]
        [Display(Name = "Kĩ Năng")]
        public string Name { get; set; }

        public ICollection<PostSkills> PostSkills { get; set; }
        public ICollection<CompanySkills> CompanySkills { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
