using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecruitmentApp.Models;

namespace RecruitmentApp.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int CompanyId { get; set; }

        public string Slug { get; set; }
        [Required]
        [DisplayName("Tên Công Ty")]
        public string Name { get; set; }
        public string FullName { get; set; }

        public string Size { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        //[Required]
        [Phone(ErrorMessage = "Không phải {0}")]
        public string Phone { get; set; }
        //[Required]
        [EmailAddress(ErrorMessage = "Không phải {0}")]
        public string Email { get; set; }
        //[Required]
        public string Type { get; set; }
        //[Required]
        public string Nation { get; set; }

        public string OverTime { get; set; }
        //[Required]
        public string WorkingTime { get; set; }
        //[Required]
        public string LogoImage { get; set; }
        public string LogoFullPath { get; set; }
        //[Required]
        public string CompanyUrl { get; set; }
        public string CompanyFbUrl { get; set; }

        public bool IsShowCompanyUrl { get; set; }
        public bool IsShowCompanyFbUrl { get; set; }


        public string TopReason { get; set; }
        public string OurExpertise { get; set; }
        public string WhyJoinUs { get; set; }


        public int? WorkTypeId { get; set; }
        public WorkType WorkType { get; set; }


        public int? CompanyTypeId { get; set; }
        public CompanyType CompanyType { get; set; }
        public ICollection<Post> Posts  { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<CompanySkills> CompanySkills { get; set; }
        public ICollection<Follower> Followers { get; set; }
        public ICollection<Location> Locations { get; set; }
        public ICollection<Review> Reviews { get; set; }

        //  public virtual ICollection<Skill> Skills { get; set; }

        //public virtual ICollection<AppUser> Followers { get; set; }

        public ICollection<Image> Images { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        public ICollection<CompanyIndustry> CompanyIndustries { get; set; }
        public int? CountryId { get; set; }
        public Country Country { get; set; }

        public bool IsSpotlight { get; set; } = false;
        public ICollection<AppUser> Recruiters { get; set; }
        public bool IsShowOnHome { get; set; } = false;
    }
}
