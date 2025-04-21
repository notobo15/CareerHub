using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Areas.Admin.Companies.Controllers;
using RecruitmentApp.Data;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Recruiter.Home.Controllers
{
    [Route("/recruiter")]
    [Area("Recruiter/Home")]
    //[Authorize(Roles = RoleName.Administrator)]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CompanyController> _logger;

        public HomeController(AppDbContext context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Route("dashboard")]
        public async Task<IActionResult> Index()
        {

            ViewData["users"] = await _context.Users.Take(5).ToListAsync();
            return View();
        }
    }
}