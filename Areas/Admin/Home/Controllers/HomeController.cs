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
using RecruitmentApp.Areas.Admin.Home.ViewModels;
using RecruitmentApp.Data;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.Home.Controllers
{
    [Route("/admin/[action]")]
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
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalCompanies = await _context.Companies.CountAsync();
            ViewBag.TotalPosts = await _context.Posts.CountAsync();
            return View();
        }

        [HttpGet]
        public IActionResult Settings()
        {

            var setting = _context.Settings.FirstOrDefault();

            if (setting == null)
            {
                setting = new Setting();
                _context.Settings.Add(setting);
                _context.SaveChanges();
            }

            return View(setting);
        }
        // GET: Admin/Setting/Edit/1
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = _context.Settings.FirstOrDefault(s => s.Id == id);

            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // POST: Admin/Setting/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Setting model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var setting = _context.Settings.FirstOrDefault(s => s.Id == id);
                    if (setting == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật lại các field
                    setting.NumberOfPosts = model.NumberOfPosts;
                    setting.NumberOfCompanies = model.NumberOfCompanies;
                    setting.PhoneNumber = model.PhoneNumber;
                    setting.Email = model.Email;
                    setting.TaxNumber = model.TaxNumber;

                    _context.Update(setting);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Cập nhật Setting thành công!";
                    return RedirectToAction(nameof(Settings)); // hoặc RedirectToAction("Settings") tùy bạn
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật dữ liệu.");
                }
            }

            return View(model);
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