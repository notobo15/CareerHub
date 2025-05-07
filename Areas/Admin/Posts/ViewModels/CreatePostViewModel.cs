using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System;
using RecruitmentApp.Validation;

namespace RecruitmentApp.Areas.Admin.Posts.ViewModels
{
    public class CreatePostViewModel
    {
        public int? PostId { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Tên Công Việc")]
        public string Title { get; set; }

        public string Slug { get; set; }
        public bool IsHot { get; set; }
        public string Salary { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Lương tối thiểu phải ≥ 0")]
        public double MinSalary { get; set; }
        [ValidateMaxSalary]
        public double MaxSalary { get; set; }

        public string SalaryType { get; set; }
        public string SalaryText { get; set; }

        public DateTime PostDate { get; set; } = DateTime.Now;
        public DateTime Expired { get; set; } = DateTime.Now;

        public string WorkSpace { get; set; }

        public int? AddressId { get; set; }

        public string Description { get; set; }
        public string JobRequirement { get; set; }
        public string Benefit { get; set; }

        public int? CompanyId { get; set; }
        public string RecruiterId { get; set; }

        public bool IsShow { get; set; } = true;
        public bool IsClose { get; set; }
        public int LocationId { get; set; }
        public string TopReason { get; set; }
        public string DegreeRequirement { get; set; }

        public int Quantity { get; set; }

        // --- Selectable IDs ---
        public int[] SkillIds { get; set; }
        public int[] WorkTypeIds { get; set; }
        public int[] LevelIds { get; set; }
        public int[] TitleIds { get; set; }
        public int[] IndustryIds { get; set; }
        public int[] LocationIds { get; set; }
        // --- Dropdown data ---
        public MultiSelectList Skills { get; set; }
        public MultiSelectList WorkTypes { get; set; }
        public MultiSelectList Levels { get; set; }
        public MultiSelectList Titles { get; set; }
        public MultiSelectList Industries { get; set; }
        public MultiSelectList Locations { get; set; }
        public SelectList Recruiters { get; set; }
        public SelectList Companies { get; set; }
        public SelectList SalaryTypes { get; set; }

        public string salaryToString()
        {
            if (SalaryType?.ToLower() == "custom")
            {
                return "You'll love it";
            }
            else if (SalaryType?.ToLower() == "range" && MinSalary != 0 && MaxSalary != 0)
            {
                return $"{MinSalary:#,##0.###} - {MaxSalary:#,##0.###} VNĐ";
            }
            else if (SalaryType?.ToLower() == "up_to")
            {
                return $"Up to {MaxSalary:#,##0.###} gross";
            }

            return "Salary not specified";
        }
    }
}
