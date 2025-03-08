using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RecruitmentApp.DTOs;
using RecruitmentApp.Models;
using RecruitmentApp.Services.Users;
using RecruitmentApp.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecruitmentApp.Areas.User.Jobs.Controllers
{
    [Area("User/Jobs")]

    [Route("/jobs")]
    [Route("/viec-lam")]
    public class JobController : Controller
    {

        private readonly AppDbContext _dbContext;
        private readonly PostService _postService;

        public JobController(AppDbContext context, PostService postService)
        {
            _dbContext = context;
            _postService = postService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            var industries = _dbContext.Industries.ToList();
            var posts = _dbContext.Posts
              .Include(p => p.PostSkills)
              .ThenInclude(pk => pk.Skill)
              .Include(p => p.Company)

              .Include(p => p.Location)
              .ThenInclude(pl => pl.Address)
              .ThenInclude(pla => pla.City)

              .ToList();


            ViewData["industries"] = industries;
            return View();
        }
        [HttpGet]
        [Route("{slug}")]
        public IActionResult Detail(string slug)
        {
            var post = _dbContext.Posts
                .Include(p => p.Company)
                .Include(p => p.PostSkills)
                .ThenInclude(ps => ps.Skill)

                .Include(p => p.Company)
                .ThenInclude(pc => pc.CompanyIndustries)
                .ThenInclude(pc => pc.Industry)

                .Include(p => p.PostLocations)
                .ThenInclude(pl => pl.Location)
                .ThenInclude(pl => pl.Address)

                .SingleOrDefault(p => p.Slug == slug);

            ViewData["post"] = post;

            return View();
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string province = "0", string key = "all", int page = 1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int pageSize = 10; // Số lượng bài đăng trên mỗi trang

            var industries = _dbContext.Industries.ToList();

            var query = _dbContext.Posts
                .Include(p => p.PostSkills)
                .ThenInclude(pk => pk.Skill)

                .Include(p => p.Company)
                .ThenInclude(cc => cc.Country)

                .Include(p => p.Company)
                .ThenInclude(cc => cc.CompanyIndustries)
                .ThenInclude(ci => ci.Industry)


                .Include(p => p.PostLocations)
                .ThenInclude(pl => pl.Location)
                .ThenInclude(pla => pla.Address)
                .ThenInclude(pla => pla.City)

                .OrderByDescending(p => p.PostDate)

                .AsQueryable();

            // Chỉ lọc nếu key KHÔNG phải "all"
            if (!string.IsNullOrEmpty(key) && key != "all")
            {
                query = query.Where(p =>
                    p.Title.Contains(key) ||
                    p.Description.Contains(key) ||
                    p.Company.Name.Contains(key) ||
                    p.PostSkills.Any(ps => ps.Skill.Name.Contains(key))
                );
            }

            int totalPosts = query.Count(); // Tổng số bài đăng tìm được
            var posts = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var postDTOs = posts.Select(p => new PostDTO
            {
                PostId = p.PostId,
                Title = p.Title,
                Slug = p.Slug,
                IsHot = p.IsHot,
                ViewTotal = p.ViewTotal,
                Salary = p.Salary,
                SalaryText = p.salaryToString(),
                PostDate = p.PostDate,
                Expired = p.Expired,
                WorkSpace = p.WorkSpace,
                TimeAgo = TimeFormatter.GetTimeAgo(p.PostDate),

                Description = p.Description,
                Benifit = p.Benifit,
                DegreeRequirement = p.DegreeRequirement,
                JobRequirement = p.JobRequirement,
                TopReason = p.TopReason,
                Company = p.Company != null ? new CompanyDTO
                {
                    CompanyId = p.Company.CompanyId,
                    Name = p.Company.Name,
                    Slug = p.Company.Slug,
                    Size = p.Company.Size,
                    Description = p.Company.Description,
                    Phone = p.Company.Phone,
                    Email = p.Company.Email,
                    Type = p.Company.Type,
                    WorkingTime = p.Company.WorkingTime,
                    LogoImage = p.Company.LogoImage,
                    Country = p.Company.Country.Name,
                    CountryCode = p.Company.Country.ISOCode,
                    ShortDescription = p.Company.ShortDescription,
                    CompanyUrl = p.Company.CompanyUrl,
                    Skills = p.Company.CompanySkills?.Select(cs => cs.Skill.Name).ToList(),
                    Locations = p.Company.Locations?.Select(l => l.Address.City.Name).ToList(),
                    Industries = p.Company.CompanyIndustries.Select(ps => new IndustryDTO
                    {
                        IndustryId = ps.Industry.IndustryId,
                        Name = ps.Industry.Name,
                    }).ToList(),
                } : null,
                Skills = p.PostSkills.Select(ps => new SkillDTO
                {
                    SkillId = ps.Skill.SkillId,
                    Name = ps.Skill.Name,
                    Slug = ps.Skill.Slug
                }).ToList(),

                Addresses = p.PostLocations != null ? p.PostLocations.Select(pl => new AddressDTO
                {
                    AddressId = pl.Location.Address.AddressId,
                    Nation = pl.Location.Address.City?.Name,
                    ProvinceCode = pl.Location.Address.City?.CodeName,
                    Province = pl.Location.Address.City?.FullName,
                    DetailPosition = pl.Location.Address.DetailPosition,
                    GgMapSrc = pl.Location.Address.GgMapSrc,
                    FullAddress = pl.Location.Address.FullAddress
                }).ToList() : new List<AddressDTO>()

            }).ToList();
            ViewData["industries"] = industries;
            ViewData["posts"] = postDTOs;
            ViewData["currentPage"] = page;
            ViewData["totalPages"] = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewData["province"] = province;
            ViewData["totalPosts"] = totalPosts;
            ViewData["key"] = key;
            ViewData["post"] = postDTOs.FirstOrDefault();

            return View();
        }

        [HttpGet]
        [Route("detail")]
        public IActionResult GetJobDetail(int postId)
        {
            var postDTOs = _postService.GetById(postId);

            if (postDTOs == null)
            {
                return NotFound();
            }
            return ViewComponent("JobDetail", new { post = postDTOs });
        }
    }
}
