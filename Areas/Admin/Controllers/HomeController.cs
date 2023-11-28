using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Areas.Companies.Controllers;
using RecruitmentApp.Data;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin")]
    [Authorize(Roles = RoleName.Administrator)]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CompanyController> _logger;

        public HomeController(AppDbContext context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["totalUsers"]  = _context.Users.Count() | 0;
            ViewData["totalCompanies"]  = _context.Companies.Count() | 0;
            ViewData["totalPosts"]  = _context.Posts.Count() | 0;
            ViewData["totalApplies"]  = _context.applyPosts.Count() | 0;
            ViewData["listTotalCompanyType"] = new List<int>
            {
                _context.Companies.Where(c => c.Type == "Product").Count(),
                _context.Companies.Where(c => c.Type == "Outsourcing").Count(),
            };
            var nations = _context.Companies.Select(c => c.Nation).ToList();
            List<string> uniqueNations = nations.Distinct().ToList();

            List<int> nationCounts = uniqueNations.Select(n => nations.Count(x => x == n)).ToList();
            ViewData["uniqueNations"] = uniqueNations;
            ViewData["nationCounts"] = nationCounts;


            var PostSkills = _context.PostSkills
                .Include(e => e.Skill)
                .Select(c => c.Skill.Name).ToList();
            List<string> uniqueSkills = PostSkills.Distinct().ToList();

            List<int> SkillCounts = uniqueSkills.Select(n => PostSkills.Count(x => x == n)).ToList();
            ViewData["uniqueSkills"] = uniqueSkills;
            ViewData["SkillCounts"] = SkillCounts;
            var address = _context.Posts.Include(p => p.Address).Select(p => p.Address.ProvinceCode).ToList();
            var hanoi = address.Where(c => c == "01").Count();
            var danang = address.Where(c => c == "48").Count();
            var hcm = address.Where(c => c == "79").Count();
            var other = address.Count - ( hanoi + danang + hcm);
            ViewData["address"] = new List<int>
            {
              hanoi,
              danang,
              hcm,
             other
            };

            return View();
        }
    }
}