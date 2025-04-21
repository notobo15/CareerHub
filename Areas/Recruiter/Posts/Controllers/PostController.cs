using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bogus.DataSets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Areas.Admin.Posts.ViewModels;
using RecruitmentApp.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RecruitmentApp.Areas.Recruiter.Posts.Controllers
{
    [Area("Recruiter/Posts")]
    [Route("/recruiter/[controller]/[action]/{id?}")]
    public class PostController : Controller

    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PostController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> GetUserId()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }

        // GET: Post
        public async Task<IActionResult> Index()
        {
            var userId = await GetUserId();


            return View(await _context.Posts.Where(p => p.RecruiterId == userId).ToListAsync());
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Post/Create
        public IActionResult Create()
        {

            var listSkill = _context.Skills.ToList();
            ViewData["list"] = new MultiSelectList(listSkill, "SkillId", "Name");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostSkills)
                .Include(p => p.PostLevels)
                .Include(p => p.PostTitles)
                .Include(p => p.PostIndustries)
                .Include(p => p.PostWorkTypes)
                .Include(p => p.PostLocations)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null) return NotFound();

            var vm = new CreatePostViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Slug = post.Slug,
                MinSalary = post.MinSalary,
                MaxSalary = post.MaxSalary,
                SalaryType = post.SalaryType,
                SalaryText = post.SalaryText,
                TopReason = post.TopReason,
                Description = post.Description,
                JobRequirement = post.JobRequirement,
                Benefit = post.Benefit,
                Quantity = post.Quantity,
                CompanyId = post.CompanyId,
                RecruiterId = post.RecruiterId,
                AddressId = post.AddressId,
                PostDate = post.PostDate,
                Expired = post.Expired,
                WorkSpace = post.WorkSpace,
                IsHot = post.IsHot,
                IsShow = post.IsShow,
                IsClose = post.IsClose,
                SkillIds = post.PostSkills.Select(x => x.SkillID).ToArray(),
                LevelIds = post.PostLevels.Select(x => x.LevelID).ToArray(),
                TitleIds = post.PostTitles.Select(x => x.TitleId).ToArray(),
                IndustryIds = post.PostIndustries.Select(x => x.IndustryId).ToArray(),
                WorkTypeIds = post.PostWorkTypes.Select(x => x.WorkTypeId).ToArray(),
                LocationIds = post.PostLocations.Select(x => x.LocationId).ToArray(),
            };

            // Dữ liệu dropdown 
            await LoadDropdownsAsync(vm);

            return View(vm);
        }

        async Task LoadDropdownsAsync(CreatePostViewModel model)
        {
            model.Skills = new MultiSelectList(_context.Skills, "SkillId", "Name", model.SkillIds);
            model.Levels = new MultiSelectList(_context.Levels, "LevelId", "Name", model.LevelIds);
            model.Titles = new MultiSelectList(_context.Titles, "Id", "Name", model.TitleIds);
            model.Industries = new MultiSelectList(_context.Industries, "IndustryId", "Name", model.IndustryIds);
            model.WorkTypes = new MultiSelectList(_context.WorkTypes, "WorkTypeId", "Name", model.WorkTypeIds);
            model.Companies = new SelectList(_context.Companies, "CompanyId", "Name", model.CompanyId);
            model.Recruiters = new SelectList(await _userManager.GetUsersInRoleAsync("Recruiter"), "Id", "UserName", model.RecruiterId);
            model.SalaryTypes = new SelectList(new[] {
                new { Value = "custom", Text = "Custom" },
                new { Value = "range", Text = "Range" },
                new { Value = "up_to", Text = "Up To" }
                }, "Value", "Text", model.SalaryType);

            if (model.CompanyId.HasValue)
            {
                var locations = _context.Locations
                    .Where(l => l.CompanyId == model.CompanyId)
                    .Select(l => new
                    {
                        l.LocationId,
                        FullAddress = l.Address.FullAddress
                    })
                    .ToList();

                model.Locations = new SelectList(locations, "LocationId", "FullAddress", model.LocationId);
            }
            else
            {
                model.Locations = new SelectList(Enumerable.Empty<object>(), "LocationId", "FullAddress");
            }

        }



        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreatePostViewModel model)
        {
            if (id != model.PostId)
                return NotFound();

            var post = await _context.Posts
                .Include(p => p.PostLevels)
                .Include(p => p.PostSkills)
                .Include(p => p.PostTitles)
                .Include(p => p.PostIndustries)
                .Include(p => p.PostWorkTypes)
                .Include(p => p.PostLocations)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
                return NotFound();

            if (model.MinSalary > model.MaxSalary)
            {
                ModelState.AddModelError("MinSalary", "Lương tối thiểu không được lớn hơn lương tối đa.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(model); // method tái sử dụng để load các SelectList
                return View(model);
            }

            try
            {
                post.AddressId = model.AddressId;
                post.Title = model.Title;
                post.Slug = model.Slug;
                post.IsHot = model.IsHot;
                post.IsShow = model.IsShow;
                post.IsClose = model.IsClose;
                post.MinSalary = model.MinSalary;
                post.MaxSalary = model.MaxSalary;
                post.SalaryType = model.SalaryType;
                post.SalaryText = model.SalaryText;
                post.Quantity = model.Quantity;
                post.WorkSpace = model.WorkSpace;
                post.Description = model.Description;
                post.JobRequirement = model.JobRequirement;
                post.Benefit = model.Benefit;
                post.TopReason = model.TopReason;
                post.Expired = model.Expired;
                post.RecruiterId = model.RecruiterId;
                post.CompanyId = model.CompanyId;
                post.LocationId = model.LocationId;
                post.UpdatedAt = DateTime.Now;

                // Skills
                var oldSkillIds = post.PostSkills.Select(x => x.SkillID).ToList();
                var newSkillIds = model.SkillIds ?? new int[] { };

                _context.PostSkills.RemoveRange(post.PostSkills.Where(x => !newSkillIds.Contains(x.SkillID)));
                foreach (var sid in newSkillIds.Except(oldSkillIds))
                {
                    post.PostSkills.Add(new PostSkills { PostID = id, SkillID = sid });
                }

                // Levels
                var oldLevelIds = post.PostLevels.Select(x => x.LevelID).ToList();
                var newLevelIds = model.LevelIds ?? new int[] { };

                _context.PostLevels.RemoveRange(post.PostLevels.Where(x => !newLevelIds.Contains(x.LevelID)));
                foreach (var lid in newLevelIds.Except(oldLevelIds))
                {
                    post.PostLevels.Add(new PostLevel { PostID = id, LevelID = lid });
                }

                // Titles
                var oldTitleIds = post.PostTitles.Select(x => x.TitleId).ToList();
                var newTitleIds = model.TitleIds ?? new int[] { };

                _context.PostTitles.RemoveRange(post.PostTitles.Where(x => !newTitleIds.Contains(x.TitleId)));
                foreach (var tid in newTitleIds.Except(oldTitleIds))
                {
                    post.PostTitles.Add(new PostTitle { PostId = id, TitleId = tid });
                }

                // Industries
                var oldIndustryIds = post.PostIndustries.Select(x => x.IndustryId).ToList();
                var newIndustryIds = model.IndustryIds ?? new int[] { };

                _context.PostIndustries.RemoveRange(post.PostIndustries.Where(x => !newIndustryIds.Contains(x.IndustryId)));
                foreach (var iid in newIndustryIds.Except(oldIndustryIds))
                {
                    post.PostIndustries.Add(new PostIndustry { PostId = id, IndustryId = iid });
                }

                // WorkTypes
                var oldWorkTypeIds = post.PostWorkTypes.Select(x => x.WorkTypeId).ToList();
                var newWorkTypeIds = model.WorkTypeIds ?? new int[] { };

                _context.PostWorkTypes.RemoveRange(post.PostWorkTypes.Where(x => !newWorkTypeIds.Contains(x.WorkTypeId)));
                foreach (var wid in newWorkTypeIds.Except(oldWorkTypeIds))
                {
                    post.PostWorkTypes.Add(new PostWorkType { PostId = id, WorkTypeId = wid });
                }

                _context.Update(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật bài đăng.");
                await LoadDropdownsAsync(model);
                return View(model);
            }
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        [HttpPost]
        public IActionResult Favourite(string userId, int? postId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", controllerName: "Account", new { area = "Identity" });
            }
            if (postId != null)
            {
                _context.Favorites.Add(new Favorite
                {
                    PostID = (int)postId,
                    UserID = userId
                });
            }
            _context.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public IActionResult UnFavourite(string userId, int? postId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", controllerName: "Account", new { area = "Identity" });
            }
            if (postId != null)
            {
                var follower = _context.Favorites.FirstOrDefault(e => e.PostID == postId && e.UserID == userId);
                _context.Favorites.Remove(follower);
            }
            _context.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Applicants(int id)
        {
            var post = await _context.Posts
                .Include(p => p.ApplyPosts)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null) return NotFound();
            return View(post);
        }


        public async Task<IActionResult> ApplicantDetails(int id)
        {
            var application = await _context.ApplyPosts
                .Include(a => a.User)
                .Include(a => a.Post)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            return View(application);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ApplyPost(int postId, IFormFile uploadedFile, string name, string phone, string description, bool useCurrentCv)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null) return Unauthorized();

        //    string fileName = null;
        //    string originalName = null;
        //    string filePath = null;

        //    if (useCurrentCv)
        //    {
        //        var resume = await _context.ResumeFiles
        //            .FirstOrDefaultAsync(r => r.UserId == user.Id && r.IsPrimary);

        //        if (resume == null)
        //        {
        //            ModelState.AddModelError("", "Bạn chưa có CV mặc định.");
        //            return RedirectToAction("Details", "Post", new { id = postId });
        //        }

        //        fileName = resume.FileName;
        //        originalName = resume.OrigialFileName;
        //        filePath = resume.FilePath;
        //    }
        //    else
        //    {
        //        if (uploadedFile != null && uploadedFile.Length > 0)
        //        {
        //            var fileExt = Path.GetExtension(uploadedFile.FileName);
        //            var newFileName = Guid.NewGuid().ToString() + fileExt;
        //            var path = Path.Combine(_env.WebRootPath, "uploads/cv", newFileName);

        //            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        //            using (var stream = new FileStream(path, FileMode.Create))
        //            {
        //                await uploadedFile.CopyToAsync(stream);
        //            }

        //            fileName = newFileName;
        //            originalName = uploadedFile.FileName;
        //            filePath = "/uploads/cv/" + newFileName;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Vui lòng chọn file CV.");
        //            return RedirectToAction("Details", "Post", new { id = postId });
        //        }
        //    }

        //    var apply = new ApplyPost
        //    {
        //        PostID = postId,
        //        UserID = user.Id,
        //        Name = name,
        //        Phone = phone,
        //        Description = description,
        //        FileName = fileName,
        //        OriginFileName = originalName,
        //        FilePath = filePath,
        //        ApplyDate = DateTime.Now
        //    };

        //    _context.ApplyPosts.Add(apply);
        //    await _context.SaveChangesAsync();

        //    TempData["Success"] = "Bạn đã ứng tuyển thành công.";
        //    return RedirectToAction("Details", "Post", new { id = postId });
        //}

        [Route("admin/post/applicant-user")]
        public async Task<IActionResult> ApplicantUserDetail(string id)
        {
            //if (string.IsNullOrEmpty(id))
            //    return BadRequest("User ID is required.");

            //var user = await _userManager.Users
            //    .Include(u => u.ResumeFiles) // nếu bạn muốn show CV
            //    .FirstOrDefaultAsync(u => u.Id == id);

            //if (user == null)
            //    return NotFound("User not found.");
            var applicant = await _context.ApplyPosts
               .Include(a => a.User)
               .Include(a => a.Post)
            .FirstOrDefaultAsync(a => a.UserID == id);

            if (applicant == null) return NotFound();

            return View(applicant);
        }



        // API route
        [HttpGet]
        public IActionResult GetLocationsByRecruiter(string recruiterId)
        {
            if (string.IsNullOrEmpty(recruiterId))
                return BadRequest("RecruiterId is required.");

            var company = _context.Companies
                .Include(c => c.Locations)
                    .ThenInclude(l => l.Address)
                .FirstOrDefault(c => c.Recruiters.Any(r => r.Id == recruiterId)); // hoặc c.CompanyId == user.CompanyId

            if (company == null)
                return NotFound("Company not found for recruiter.");

            var locations = company.Locations.Select(loc => new
            {
                loc.LocationId,
                FullAddress = loc.Address?.FullAddress ?? "(Địa chỉ chưa đầy đủ)"
            }).ToList();

            return Json(locations);
        }
    }
}
