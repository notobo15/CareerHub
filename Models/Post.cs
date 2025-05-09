﻿using RecruitmentApp.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }

        [Required]
        [Display(Name = "Tên Công Việc")]
        [StringLength(250)]
        public string Title { get; set; }

        public string Slug { get; set; }
        public bool IsHot { get; set; }
        public int ViewTotal { get; set; }
        //public string Salary { get; set; }

        [Range(0, int.MaxValue)]
        public double MinSalary { get; set; }
        [ValidateMaxSalary]
        public double MaxSalary { get; set; }

        public string SalaryType { get; set; }
        public string SalaryText { get; set; }

        public DateTime PostDate { get; set; }
        public DateTime Expired { get; set; }
        public string WorkSpace { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [Column(TypeName = "ntext")]
        public string JobRequirement { get; set; }

        [Column(TypeName = "ntext")]
        public string Benefit { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public string RecruiterId { get; set; }
        public AppUser AppUser { get; set; }

        public bool IsShow { get; set; } = true;
        public bool IsClose { get; set; }

        public string TopReason { get; set; }
        public string DegreeRequirement { get; set; }

        public int Quantity { get; set; }

        //public virtual ICollection<Skill> Skills { get; set; }
        // public virtual ICollection<Title> Titles { get; set; }
        // public virtual ICollection<Level> Levels { get; set; }
        // public virtual ICollection<Company> Companies { get; set; }

        public ICollection<PostSkills> PostSkills { get; set; }
        public ICollection<PostWorkType> PostWorkTypes { get; set; }
        public ICollection<PostLevel> PostLevels { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<PostLocations> PostLocations { get; set; }
        public ICollection<PostIndustry> PostIndustries { get; set; }
        public ICollection<ApplyPost> ApplyPosts { get; set; }
        // public virtual ICollection<AppUser> Favourites { get; set; }

        public int? LocationId { get; set; }
        public Location Location { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        public int[] SkillIds { get; set; }

        [NotMapped]
        public int[] LevelIds { get; set; }


        public bool IsShowOnHome { get; set; } = false;
        public ICollection<PostTitle> PostTitles { get; set; }


        // public string salaryToString()
        // {
        //     if (MinSalary != 0 && MaxSalary != 0)
        //     {
        //         return $"{MinSalary.ToString("#,##0.###")} - {MaxSalary.ToString("#,##0.###")} VND";
        //     }
        //     else if (MinSalary == 0)
        //     {
        //         return $"Up To {MaxSalary.ToString("#,##0.###")} VND";
        //     }
        //     else
        //     {
        //         return $"Min {MinSalary.ToString("#,##0.###")} VND";
        //     }
        // }
        public string salaryToString()
        {
            if (SalaryType?.ToLower() == "custom")
            {
                return "You'll love it";
            }
            else if (SalaryType?.ToLower() == "range")
            {
                if (MinSalary != 0 && MaxSalary != 0)
                {
                    return $"{MinSalary.ToString("#,##0.###")} - {MaxSalary.ToString("#,##0.###")} VNĐ";
                }
            }
            else if (SalaryType?.ToLower() == "up_to")
            {
                return $"Up to {MaxSalary.ToString("#,##0.###")} gross";
            }

            return "Salary not specified";
        }
    }
}
