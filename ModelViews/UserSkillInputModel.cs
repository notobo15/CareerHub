using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.ModelViews
{
    public class UserSkillInputModel
    {
        [Required(ErrorMessage = "Vui lòng chọn kỹ năng.")]
        public int SkillId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cấp bậc.")]
        public string SkillLevel { get; set; }
    }
}
