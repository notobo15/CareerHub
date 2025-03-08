using System.Collections.Generic;
using System;

namespace RecruitmentApp.DTOs
{
    public class CompanyDTO
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string LogoImage { get; set; }
        public string CompanyUrl { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string WorkingTime { get; set; }
        public string ShortDescription { get; set; }

        public List<string> Skills { get; set; }
        public List<string> Locations { get; set; }
        public List<IndustryDTO> Industries { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
