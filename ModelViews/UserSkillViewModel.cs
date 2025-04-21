using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.ModelViews
{
    public class UserSkillViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kỹ năng.")]
        [Display(Name = "Skill")]
        public int SkillId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn mức độ.")]
        [Display(Name = "Skill Level")]
        public string SkillLevel { get; set; } // Beginner, Intermediate, Excellent

        public string? SkillName { get; set; }
    }
    public enum SkillLevel
    {
        Beginner,
        Intermediate,
        Excellent
    }
}
