using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Areas.Admin.Companies.Controllers;
using RecruitmentApp.Data;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.Home.Controllers
{
    [Route("/admin")]
    [Area("Admin/Home")]
    [Authorize]
    [Authorize(Roles = RoleName.Admin)]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CompanyController> _logger;

        public HomeController(AppDbContext context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {

            ViewData["users"] = await _context.Users.Take(5).ToListAsync();
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