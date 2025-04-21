using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.ModelViews
{
    public class GeneralInfoViewModel
    {
        public string? YearsOfExperience { get; set; }
        public int? LevelId { get; set; }
        public int? WorkTypeId { get; set; }
        public decimal? MinExpectedSalary { get; set; }
        public decimal? MaxExpectedSalary { get; set; }
        public decimal? CurrentSalary { get; set; }

        [Required]
        [MaxLength(5, ErrorMessage = "Bạn chỉ được chọn tối đa 5 ngành.")]
        public List<int> SelectedIndustryIds { get; set; }

        public MultiSelectList Industries { get; set; }
        public SelectList Levels { get; set; }
        public SelectList WorkTypes { get; set; }
    }
}
