using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus.DataSets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RecruitmentApp.Models;
using RecruitmentApp.Services.Users;

namespace RecruitmentApp.Areas.User.Home.Controllers
{
    [Area("User/Home")]
    [Route("")]

    public class HomeController : Controller
    {

        private readonly AppDbContext _dbContext;
        private readonly CompanyService _companyService;
        private readonly PostService _postService;
        public HomeController(AppDbContext dbContext, CompanyService companyService, PostService postService)
        {
            _dbContext = dbContext;
            _companyService = companyService;
            _postService = postService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var setting = _dbContext.Settings.FirstOrDefault();

            int numberOfCompanies = setting?.NumberOfCompanies ?? 8;
            int numberOfPosts = setting?.NumberOfPosts ?? 8;

            var companies = await _companyService.GetTopCompaniesAsync(numberOfCompanies);
            var posts = await _postService.GetLatestPostsAsync(numberOfPosts);

            ViewData["Title"] = "CareerHub | Việc làm IT Nhất Dành Cho Bạn";
            ViewData["companies"] = companies;
            ViewData["posts"] = posts;
            ViewData["totalPosts"] = await _dbContext.Posts.CountAsync();
            ViewData["blogs"] = await _dbContext.Blogs.Include(b => b.Category).OrderBy(b => b.UpdatedAt).Take(5).Where(b => b.IsShowOnHome).ToListAsync();
            return View();
        }
        [HttpPost]
        [Route("set-language")]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        [Route("condition")]
        public IActionResult Condition()
        {
            return View();
        }
        [Route("policy")]
        public IActionResult Policy()
        {
            return View();
        }


    }
}
