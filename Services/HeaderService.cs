using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Models;
using System.Linq;
using RecruitmentApp.ModelViews;
using RecruitmentApp.DTOs;

namespace RecruitmentApp.Services
{
    public class HeaderService
    {

        private readonly ILogger<HeaderService> _logger;
        private readonly AppDbContext _dbContext;

        public HeaderService(AppDbContext dbContext, ILogger<HeaderService> logger)
        {

            _dbContext = dbContext;
            _logger = logger;
        }

        public List<CompanyDto> GetMenuCompanies()
        {
            return _dbContext.Companies
               .Select(c => new CompanyDto
               {
                   Name = c.Name,
                   Slug = c.Slug
               })
                .OrderBy(c => c.Name)
                .Take(8 * 3)
                .ToList();
        }
        public List<TitleDto> GetMenuTitles()
        {
            return _dbContext.Titles
               .Select(c => new TitleDto
               {
                   Name = c.Name,
                   Slug = c.Slug
               })
                .OrderBy(c => c.Name)
               .Take(8 * 3)
               .ToList();
        }
        public List<SkillDto> GetMenuSkills()
        {
            return _dbContext.Skills
               .Select(c => new SkillDto
               {
                   SkillId = c.SkillId,
                   Name = c.Name,
                   Slug = c.Slug
               })
               .Take(8 * 3)
               .ToList();
        }
        public List<IndustryDto> GetMenuIndustries()
        {
            return _dbContext.Industries
               .Select(c => new IndustryDto
               {
                   Id = c.IndustryId,
                   Name = c.Name,
               })
               .ToList();
        }

        public List<LevelDTO> GetMenuLevels()
        {
            return _dbContext.Levels
               .Select(c => new LevelDTO
               {
                   LevelId = c.LevelId,
                   Name = c.Name,
                   Slug = c.Slug
               })
               .ToList();
        }

        public List<WorkTypeDTO> GetMenuWorkTypes()
        {
            return _dbContext.WorkTypes
               .Select(c => new WorkTypeDTO
               {
                   WorkTypeId = c.WorkTypeId,
                   Name = c.Name,
               })
               .ToList();
        }

        public Dictionary<string, string> GetMenuYearsOfExperience()
        {

            return new Dictionary<string, string>
            {
                { "less_than_1_year", "Less than 1 year" },
                { "one_year", "1 year" },
                { "two_years", "2 years" },
                { "three_years", "3 years" },
                { "four_years", "4 years" },
                { "five_years", "5 years" },
                { "six_years", "6 years" },
                { "seven_years", "7 years" },
                { "eight_years", "8 years" },
                { "nine_years", "9 years" },
                { "ten_years", "10 years" },
                { "more_than_10_years", "More than 10 years" }
            };
        }

        public List<ProvinceDto> GetMenuProvinces()
        {
            var priorityProvinces = new[] { "1", "79", "48" };

            var provinces = _dbContext.Provinces
                .Select(p => new ProvinceDto
                {
                    Code = p.Code,
                    Name = p.Name,
                    Slug = p.Slug
                })
                .ToList();

            // Tách thành phố ưu tiên và các thành phố còn lại
            var prioritizedList = provinces.Where(p => priorityProvinces.Contains(p.Code)).ToList();
            var otherProvinces = provinces.Where(p => !priorityProvinces.Contains(p.Code)).OrderBy(p => p.Name).ToList();

            // Thêm "Tất cả thành phố" lên đầu
            var finalList = new List<ProvinceDto>
            {
                new ProvinceDto
                {
                    Code = "0",
                    Name = "Tất cả thành phố",
                    Slug = "all"
                }
            };

            finalList.AddRange(prioritizedList);
            finalList.AddRange(otherProvinces);

            return finalList;
        }


    }
}
