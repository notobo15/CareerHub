using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Skill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

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
