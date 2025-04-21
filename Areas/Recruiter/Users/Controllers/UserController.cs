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

namespace RecruitmentApp.Areas.Recruiter.Users.Controllers
{

    [Area("Recruiter/Users")]
    [Route("/recruiter/user/[action]/{id}")]
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
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage)
        {
            var model = new UserListModel();
            model.currentPage = currentPage;

            var qr = _userManager.Users.OrderBy(u => u.UserName);

            model.totalUsers = await qr.CountAsync();
            model.countPages = (int)Math.Ceiling((double)model.totalUsers / model.ITEMS_PER_PAGE);

            if (model.currentPage < 1)
                model.currentPage = 1;
            if (model.currentPage > model.countPages)
                model.currentPage = model.countPages;

            var qr1 = qr.Skip((model.currentPage - 1) * model.ITEMS_PER_PAGE)
                        .Take(model.ITEMS_PER_PAGE)
                        .Select(u => new UserAndRole()
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                        });

            model.users = await qr1.ToListAsync();

            foreach (var user in model.users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }

            return View(model);
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
    }
}
