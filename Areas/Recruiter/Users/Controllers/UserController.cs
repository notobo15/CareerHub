// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RecruitmentApp.Areas.Admin.Identity.Models.AccountViewModels;
using RecruitmentApp.Areas.Admin.Identity.Models.ManageViewModels;
using RecruitmentApp.Areas.Admin.Identity.Models.RoleViewModels;
using RecruitmentApp.Areas.Admin.Identity.Models.UserViewModels;
using RecruitmentApp.Data;
using RecruitmentApp.ExtendMethods;
using RecruitmentApp.Models;
using RecruitmentApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Areas.Admin.Identity.Models.User;
using App.Areas.Admin.Identity.Controllers;
using RecruitmentApp.Areas.Recruiter.Users.ViewModels;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace RecruitmentApp.Areas.Recruiter.Users.Controllers
{

    [Area("Recruiter/Users")]
    [Route("/recruiter/user/[action]/{id?}")]
    public class UserController : Controller
    {

        private readonly ILogger<RoleController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserController(ILogger<RoleController> logger, RoleManager<IdentityRole> roleManager, AppDbContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }



        [TempData]
        public string StatusMessage { get; set; }

        //
        // GET: /ManageUser/Index
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string search = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser?.CompanyId == null)
                return NotFound("Người dùng hiện tại không thuộc công ty nào.");

            var companyId = currentUser.CompanyId.Value;

            // Truy vấn ApplyPosts có bài đăng thuộc công ty này
            var applyQuery = _context.ApplyPosts
                .Include(a => a.User)
                .Include(a => a.Post)
                .Where(a => a.Post.CompanyId == companyId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                applyQuery = applyQuery.Where(a => a.User.Email.Contains(search));
            }

            applyQuery = applyQuery.OrderByDescending(a => a.ApplyDate);

            var totalUsers = await applyQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages > 0 ? totalPages : 1;

            // Lấy danh sách user đã apply (distinct để tránh trùng nếu 1 user apply nhiều post)
            var users = await applyQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => a.User)
                .Distinct()
                .ToListAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewData["Title"] = "Danh sách ứng viên đã apply";

            return View(users); // Trả về List<AppUser>
        }


        [HttpGet]
        public IActionResult ManageApplyJob(string id)
        {
            var result = _context.ApplyPosts.Where(a => a.UserID == id).ToList();
            return View(result);
        }
        [HttpGet]
        public IActionResult Details(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));
            return View(user);

        }

        [HttpGet]
        public async Task<IActionResult> CvList()
        {
            return await LoadCVsWithFilterAsync(null);
        }

        [HttpPost]
        public async Task<IActionResult> CvList(string prompt)
        {
            return await LoadCVsWithFilterAsync(prompt);
        }

        // Hàm chung xử lý logic
        private async Task<IActionResult> LoadCVsWithFilterAsync(string prompt)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.CompanyId == null) return NotFound();

            var companyId = currentUser.CompanyId.Value;

            var cvs = await _context.ApplyPosts
                .Include(a => a.User)
                .Include(a => a.Post)
                .Where(a => a.Post.CompanyId == companyId)
                .ToListAsync();

            var results = cvs.Select(cv => new CVFilterResult
            {
                Name = cv.Name,
                Email = cv.User?.Email ?? "",
                Phone = cv.Phone,
                FilePath = cv.FilePath,
                ApplyDate = cv.ApplyDate
            }).ToList();

            if (!string.IsNullOrWhiteSpace(prompt))
            {
                // Gửi tới Django API lọc
                using var client = new HttpClient();
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(prompt), "prompt");

                foreach (var cv in cvs)
                {
                    var filePath = Path.Combine("wwwroot", cv.FilePath.TrimStart('/'));
                    if (!System.IO.File.Exists(filePath)) continue;

                    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    form.Add(fileContent, "files", Path.GetFileName(filePath));
                }

                var response = await client.PostAsync("http://localhost:8000/api/cvfilter/filter/", form);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResults = JsonSerializer.Deserialize<JsonElement>(json);

                    foreach (var item in apiResults.GetProperty("results").EnumerateArray())
                    {
                        var filename = item.GetProperty("filename").GetString();
                        var match = results.FirstOrDefault(r => r.FilePath.EndsWith(filename));
                        if (match != null)
                        {
                            match.IsFit = item.GetProperty("is_fit").GetBoolean();
                            match.Reason = item.GetProperty("reason").GetString();
                        }
                    }
                }
            }
            ViewBag.prompt = prompt;
            return View("CvList", results);
        }

        [HttpGet]
        public async Task<IActionResult> UserApplyList(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Thiếu userId");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            var applyPosts = await _context.ApplyPosts
                .Include(a => a.Post)
                .Where(a => a.UserID == userId)
                .OrderByDescending(a => a.ApplyDate)
                .ToListAsync();

            ViewBag.UserName = user.FullName ?? user.UserName;
            ViewBag.UserEmail = user.Email;

            return View(applyPosts);
        }

    }
}
