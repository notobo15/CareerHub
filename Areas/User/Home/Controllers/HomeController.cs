using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.User.Home.Controllers
{
    [Area("User/Home")]
    [Route("/home")]
    [Route("/trang-chu")]
    public class HomeController : Controller
    {

        private readonly AppDbContext _dbContext;
        private readonly IStringLocalizer<HomeController> _localizer;
        public HomeController(AppDbContext dbContext, IStringLocalizer<HomeController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
              var companies = _dbContext.Companies
                  .Include(c => c.CompanySkills)
                  .ThenInclude(ck => ck.Skill)
                  .Include(c => c.Locations)
                  .ThenInclude(l => l.Address)
                  .ThenInclude(p => p.City)
                  //.Include(c => c.Posts)
                  //.Include(c => c.Addresses)
                  .Include(c => c.Posts)
                .OrderBy(c => c.CompanyId)
                .Distinct()
                .Take(6)
                .ToList();
            var posts = _dbContext.Posts
                .Include(p => p.PostSkills)
                .ThenInclude(pk => pk.Skill)
                .Include(p => p.Company)

                .Include(p => p.Location)
                .ThenInclude(pl => pl.Address)
                .ThenInclude(pla => pla.City)

                .Take(8)
                .ToList();

            ViewData["companies"] = companies;
            ViewData["posts"] = posts;
            ViewData["totalPosts"] = _dbContext.Posts.Count();
            return View();
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
