using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using RecruitmentApp.Models.Email;

namespace RecruitmentApp.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Avatar { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public override string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public override string Email { get; set; }

        [Display(Name = "Tên rút gọn")]
        [StringLength(50)]
        public string ShortName { get; set; }

        [Display(Name = "Giới tính")]
        [Range(0, 2)]
        public int Gender { get; set; }

        [Display(Name = "Liên kết cá nhân")]
        [Url(ErrorMessage = "Định dạng URL không hợp lệ")]
        public string? PersonalLink { get; set; }


        public string AcademicLevel { get; set; }
        public string PersonLink { get; set; }

        public string CoverLetter { get; set; }
        public string AboutMe { get; set; }


        public string? YearsOfExperience { get; set; }

        public int? LevelId { get; set; }
        public Level CurrentLevel { get; set; }

        public int? WorkTypeId { get; set; }
        public WorkType WorkType { get; set; }

        public decimal? MinExpectedSalary { get; set; }
        public decimal? MaxExpectedSalary { get; set; }
        public decimal? CurrentSalary { get; set; }

        public string? ProvinceCode { get; set; }
        public Province Province { get; set; }
        public string? Address { get; set; }
        public bool AllowJobInvitations { get; set; } = true;


        public int? CompanyId { get; set; }
        public Company Company { get; set; }


        public virtual ICollection<Post> Posts { get; set; }
        //public virtual ICollection<Post> Favourates { get; set; }
        public ICollection<Follower> Followers { get; set; }

        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ResumeFile> ResumeFiles { get; set; }
        public ICollection<PreferredLocation> PreferredLocations { get; set; }

        public ICollection<UserIndustry> UserIndustries { get; set; }
        public ICollection<Education> Educations { get; set; }
        public ICollection<WorkExperience> WorkExperiences { get; set; }
        public ICollection<UserSkill> UserSkills { get; set; }
        public ICollection<PersonalProject> PersonalProjects { get; set; }
        public ICollection<Certificate> Certificates { get; set; }
        public ICollection<Award> Awards { get; set; }
        public ICollection<BlockCompanyInvitation> BlockedCompanies { get; set; }
        public ICollection<Mail> SentMails { get; set; }
        public ICollection<MailRecipient> ReceivedMails { get; set; }
        public ICollection<Label> Labels { get; set; }
    }
}
