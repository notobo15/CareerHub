using RecruitmentApp.Models;
using System.Collections.Generic;

namespace RecruitmentApp.ModelViews
{
    public class GroupedUserSkill
    {
        public string SkillLevel { get; set; }
        public List<UserSkill> Skills { get; set; }
    }
}
