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

namespace App.Areas.Admin.Identity.Controllers
{

    [Authorize(Roles = RoleName.Admin)]
    [Area("Admin/Identity")]
    [Route("/admin/user/[action]")]
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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string search = null)
        {
            var query = _userManager.Users.OrderBy(u => u.UserName).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.UserName.Contains(search));
            }

            var totalUsers = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            // Validate page
            if (page < 1)
                page = 1;
            if (page > totalPages)
                page = totalPages > 0 ? totalPages : 1;

            var usersQuery = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserAndRole
                {
                    Id = u.Id,
                    UserName = u.UserName,
                });

            var users = await usersQuery.ToListAsync();

            // Load roles cho từng user
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }

            ViewBag.TotalUsers = totalUsers;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            ViewData["Title"] = "Danh sách Users";

            return View(users);
        }


        // GET: /ManageUser/AddRole/id
        [HttpGet("{id}")]
        public async Task<IActionResult> AddRoleAsync(string id)
        {
            // public SelectList allRoles { get; set; }
            var model = new AddUserRoleModel();
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }

            model.user = await _userManager.FindByIdAsync(id);

            if (model.user == null)
            {
                return NotFound($"Không thấy user, id = {id}.");
            }

            model.RoleNames = (await _userManager.GetRolesAsync(model.user)).ToArray<string>();

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.allRoles = new SelectList(roleNames);

            await GetClaims(model);

            return View(model);
        }

        // GET: /ManageUser/AddRole/id
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoleAsync(string id, [Bind("RoleNames")] AddUserRoleModel model)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }

            model.user = await _userManager.FindByIdAsync(id);

            if (model.user == null)
            {
                return NotFound($"Không thấy user, id = {id}.");
            }
            await GetClaims(model);

            var OldRoleNames = (await _userManager.GetRolesAsync(model.user)).ToArray();

            var deleteRoles = OldRoleNames.Where(r => !model.RoleNames.Contains(r));
            var addRoles = model.RoleNames.Where(r => !OldRoleNames.Contains(r));

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            ViewBag.allRoles = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(model.user, deleteRoles);
            if (!resultDelete.Succeeded)
            {
                ModelState.AddModelError(resultDelete);
                return View(model);
            }

            var resultAdd = await _userManager.AddToRolesAsync(model.user, addRoles);
            if (!resultAdd.Succeeded)
            {
                ModelState.AddModelError(resultAdd);
                return View(model);
            }


            StatusMessage = $"Vừa cập nhật role cho user: {model.user.UserName}";

            return RedirectToAction("Index");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SetPasswordAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }

            var user = await _userManager.FindByIdAsync(id);
            ViewBag.user = ViewBag;

            if (user == null)
            {
                return NotFound($"Không thấy user, id = {id}.");
            }

            return View();
        }

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPasswordAsync(string id, SetUserPasswordModel model)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }

            var user = await _userManager.FindByIdAsync(id);
            ViewBag.user = ViewBag;

            if (user == null)
            {
                return NotFound($"Không thấy user, id = {id}.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _userManager.RemovePasswordAsync(user);

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            StatusMessage = $"Vừa cập nhật mật khẩu cho user: {user.UserName}";

            return RedirectToAction("Index");
        }


        [HttpGet("{userid}")]
        public async Task<ActionResult> AddClaimAsync(string userid)
        {

            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy user");
            ViewBag.user = user;
            return View();
        }

        [HttpPost("{userid}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddClaimAsync(string userid, AddUserClaimModel model)
        {

            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy user");
            ViewBag.user = user;
            if (!ModelState.IsValid) return View(model);
            var claims = _context.UserClaims.Where(c => c.UserId == user.Id);

            if (claims.Any(c => c.ClaimType == model.ClaimType && c.ClaimValue == model.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã có");
                return View(model);
            }

            await _userManager.AddClaimAsync(user, new Claim(model.ClaimType, model.ClaimValue));
            StatusMessage = "Đã thêm đặc tính cho user";

            return RedirectToAction("AddRole", new { id = user.Id });
        }

        [HttpGet("{claimid}")]
        public async Task<IActionResult> EditClaim(int claimid)
        {
            var userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            var user = await _userManager.FindByIdAsync(userclaim.UserId);

            if (user == null) return NotFound("Không tìm thấy user");

            var model = new AddUserClaimModel()
            {
                ClaimType = userclaim.ClaimType,
                ClaimValue = userclaim.ClaimValue

            };
            ViewBag.user = user;
            ViewBag.userclaim = userclaim;
            return View("AddClaim", model);
        }
        [HttpPost("{claimid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClaim(int claimid, AddUserClaimModel model)
        {
            var userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            var user = await _userManager.FindByIdAsync(userclaim.UserId);
            if (user == null) return NotFound("Không tìm thấy user");

            if (!ModelState.IsValid) return View("AddClaim", model);

            if (_context.UserClaims.Any(c => c.UserId == user.Id
                && c.ClaimType == model.ClaimType
                && c.ClaimValue == model.ClaimValue
                && c.Id != userclaim.Id))
            {
                ModelState.AddModelError("Claim này đã có");
                return View("AddClaim", model);
            }


            userclaim.ClaimType = model.ClaimType;
            userclaim.ClaimValue = model.ClaimValue;

            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa cập nhật claim";


            ViewBag.user = user;
            ViewBag.userclaim = userclaim;
            return RedirectToAction("AddRole", new { id = user.Id });
        }
        [HttpPost("{claimid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClaimAsync(int claimid)
        {
            var userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            var user = await _userManager.FindByIdAsync(userclaim.UserId);

            if (user == null) return NotFound("Không tìm thấy user");

            await _userManager.RemoveClaimAsync(user, new Claim(userclaim.ClaimType, userclaim.ClaimValue));

            StatusMessage = "Bạn đã xóa claim";

            return RedirectToAction("AddRole", new { id = user.Id });
        }

        private async Task GetClaims(AddUserRoleModel model)
        {
            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == model.user.Id
                            select r;

            var _claimsInRole = from c in _context.RoleClaims
                                join r in listRoles on c.RoleId equals r.Id
                                select c;
            model.claimsInRole = await _claimsInRole.ToListAsync();


            model.claimsInUserClaim = await (from c in _context.UserClaims
                                             where c.UserId == model.user.Id
                                             select c).ToListAsync();

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
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            return View(user);
         
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                user.EmailConfirmed = false;
                _context.SaveChanges();
               
            }
             return RedirectToAction("Index");
       
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync(string role = "Recruiter")
        {
            var model = new CreateRecruiterModel
            {
                Role = role
            };

            model.Companies = await _context.Companies
                .Select(c => new SelectListItem
                {
                    Value = c.CompanyId.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            ViewData["Title"] = "Tạo Người Dùng Mới";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRecruiterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Gán vai trò Recruiter
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));

                    await _userManager.AddToRoleAsync(user, model.Role);

                    TempData["SuccessMessage"] = $"Tạo người dùng '{model.UserName}' với vai trò '{model.Role}' thành công!";
                    return RedirectToAction("Index", "User");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

    }
}
