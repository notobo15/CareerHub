using Microsoft.AspNetCore.Http;
using RecruitmentApp.Models;
using RecruitmentApp.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Areas.Companies.Models
{
    public class CreateCompany : Company
    {

        [Display(Name = "Kĩ Năng")]
        public int[] SkillIds {  get; set; }
        [ValidateImageFile(ErrorMessage = "Only image files with extensions .jpg, .jpeg, .png, and .gif are allowed.")]
        public IFormFile File { get; set; }

    }
}
