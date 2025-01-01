using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        //[Required]
        public string CompanyUrl { get; set; }
        public string CompanyFbUrl { get; set; }

        [ForeignKey("Id")]
        public string RecruiterId { get; set; }
        public AppUser Recruiter { get; set; }

        public ICollection<Post> Posts  { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<CompanySkills> CompanySkills { get; set; }
        public ICollection<Follower> Followers { get; set; }

        //  public virtual ICollection<Skill> Skills { get; set; }

        //public virtual ICollection<AppUser> Followers { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    

      
    }

    public enum CompanyType
    {
        PRODUCT, OUTSOURCE
    }

}
