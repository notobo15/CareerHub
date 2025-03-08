using System.Collections.Generic;
using System;
using RecruitmentApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.DTOs
{
    public class PostDTO
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public bool IsHot { get; set; }
        public int ViewTotal { get; set; }
        public string Salary { get; set; }
        public string TopReason { get; set; }
        public string DegreeRequirement { get; set; }
        public string SalaryText { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime Expired { get; set; }
        public string WorkSpace { get; set; }
        public string Description { get; set; }
        public string JobRequirement { get; set; }
        public string Benifit { get; set; }
        public string TimeAgo { get; set; }
        public string LocationName { get; set; }
        public List<SkillDTO> Skills { get; set; }
        public List<AddressDTO> Addresses { get; set; }
        public CompanyDTO Company { get; set; }
    }
}
