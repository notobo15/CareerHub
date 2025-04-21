using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecruitmentApp.Models;
using RecruitmentApp.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Areas.Admin.Companies.Models
{
    public class CreateCompany : Company
    {

        [Display(Name = "Kĩ Năng")]
        public int[] SkillIds {  get; set; }
        [ValidateImageFile(ErrorMessage = "Only image files with extensions .jpg, .jpeg, .png, and .gif are allowed.")]
        public IFormFile File { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> RecruiterIds { get; set; }
        public MultiSelectList Skills { get; set; }
        public CreateLocationViewModel CreateLocation { get; set; }
        public List<Location> Locations { get; set; }
    }
}
