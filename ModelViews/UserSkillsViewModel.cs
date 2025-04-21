using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecruitmentApp.ModelViews
{
    public class UserSkillsViewModel
    {
        public List<UserSkillInputModel> Skills { get; set; } = new();

        public List<GroupedUserSkill> GroupedSkills { get; set; } = new();

        public List<SelectListItem> SkillOptions { get; set; } = new();
        public List<SelectListItem> SkillLevelOptions { get; set; } = new();
    }
}
