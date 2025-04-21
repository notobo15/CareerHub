using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Data;
using RecruitmentApp.Models;
using RecruitmentApp.ModelViews;
using RecruitmentApp.Services;

namespace RecruitmentApp.Areas.User.Profiles.Controllers
{

    [Authorize]
    [Area("User/Profiles")]
    [Route("/profile")]
    public class ProfileController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;

        public ProfileController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("dashboard")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            user = await _dbContext.Users
                .Include(u => u.PreferredLocations)
                .ThenInclude(p => p.Province)

                .FirstOrDefaultAsync(u => u.Id == user.Id);

            var primaryResume = await _dbContext.ResumeFiles
           .Where(r => r.UserId == user.Id && r.IsPrimary)
           .FirstOrDefaultAsync();

            // ✅ Tổng số bài đã ứng tuyển
            var totalApplyPosts = await _dbContext.ApplyPosts
                .CountAsync(ap => ap.UserID == user.Id);

            // ✅ Tổng số bài đã lưu (favorite)
            var totalFavorites = await _dbContext.Favorites
                .CountAsync(f => f.UserID == user.Id);

            ViewData["totalApplyPosts"] = totalApplyPosts;
            ViewData["totalFavorites"] = totalFavorites;

            ViewData["User"] = user;
            ViewData["primaryResume"] = primaryResume;
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("manage-cv")]
        public async Task<IActionResult> ManageCV()
        {

            var userId = _userManager.GetUserId(User);
            var user = await _dbContext.Users
                .Include(u => u.PreferredLocations)
                .ThenInclude(p => p.Province)
                .Include(u => u.UserIndustries)
                .ThenInclude(p => p.Industry)
                  .Include(u => u.CurrentLevel)
                  .Include(u => u.WorkType)
                  .Include(u => u.UserIndustries)
                  .ThenInclude(ui => ui.Industry)
                .FirstOrDefaultAsync(u => u.Id == userId);
            var resumes = await _dbContext.ResumeFiles
           .Where(r => r.UserId == user.Id)
           .ToListAsync();

            ViewData["user"] = user;
            ViewData["resumes"] = resumes;
            ViewData["personalInfoViewModel"] = new PersonalInfoViewModel() { FullName = user.FullName, PhoneNumber = user.PhoneNumber };
            ViewData["generalInfoViewModel"] = new GeneralInfoViewModel() { CurrentSalary = user.CurrentSalary, LevelId = user.LevelId, MaxExpectedSalary = user.MaxExpectedSalary, MinExpectedSalary = user.MinExpectedSalary, WorkTypeId = user.WorkTypeId, YearsOfExperience = user.YearsOfExperience, SelectedIndustryIds = user.UserIndustries.Select(u => u.IndustryId).ToList() };
            ViewData["coverLetterViewModel"] = new CoverLetterViewModel() { CoverLetter = user.CoverLetter };

            ViewData["Title"] = "Hồ sơ đính kèm";
            return View(new ResumeUploadViewModel());
        }


        [Route("GetGeneralInfoForm")]
        public async Task<IActionResult> GetGeneralInfoForm()
        {
            var user = await _userManager.Users
                .Include(u => u.UserIndustries)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            var selectedIndustryIds = user.UserIndustries.Select(p => p.IndustryId).ToList();

            var industries = await _dbContext.Industries.ToListAsync();
            var levels = await _dbContext.Levels.ToListAsync();
            var workTypes = await _dbContext.WorkTypes.ToListAsync();

            var model = new GeneralInfoViewModel
            {
                SelectedIndustryIds = selectedIndustryIds,
                LevelId = user.LevelId,
                WorkTypeId = user.WorkTypeId,
                YearsOfExperience = user.YearsOfExperience,
                Industries = new MultiSelectList(industries, "IndustryId", "Name", selectedIndustryIds),
                Levels = new SelectList(levels, "LevelId", "Name", user.LevelId),
                WorkTypes = new SelectList(workTypes, "WorkTypeId", "Name", user.WorkTypeId)
            };

            return PartialView("ManageCV/_GeneralInfoForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SubmitGeneralInfo")]
        public async Task<IActionResult> SubmitGeneralInfo(GeneralInfoViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.Users
                .Include(u => u.UserIndustries)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (!ModelState.IsValid)
            {
                // Nạp lại danh sách chọn nếu có lỗi
                var industries = await _dbContext.Industries.ToListAsync();
                var levels = await _dbContext.Levels.ToListAsync();
                var workTypes = await _dbContext.WorkTypes.ToListAsync();

                model.Industries = new MultiSelectList(industries, "IndustryId", "Name", model.SelectedIndustryIds);
                model.Levels = new SelectList(levels, "LevelId", "Name", model.LevelId);
                model.WorkTypes = new SelectList(workTypes, "WorkTypeId", "Name", model.WorkTypeId);

                return PartialView("ManageCV/_GeneralInfoForm", model);
            }

            user.LevelId = model.LevelId;
            user.WorkTypeId = model.WorkTypeId;
            user.YearsOfExperience = model.YearsOfExperience;
            user.MinExpectedSalary = model.MinExpectedSalary;
            user.MaxExpectedSalary = model.MaxExpectedSalary;
            user.CurrentSalary = model.CurrentSalary;

            var existingIndustries = _dbContext.UserIndustries.Where(ui => ui.UserId == user.Id);
            _dbContext.UserIndustries.RemoveRange(existingIndustries);

            var newIndustries = model.SelectedIndustryIds.Select(id => new UserIndustry
            {
                UserId = user.Id,
                IndustryId = id
            });

            await _dbContext.UserIndustries.AddRangeAsync(newIndustries);

            await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Thông tin đã được cập nhật." });
        }

        [HttpGet]
        [Route("GetPersonalInfoForm")]
        public async Task<IActionResult> GetPersonalInfoForm()
        {
            var user = await _userManager.GetUserAsync(User);

            user = await _dbContext.Users
                .Include(u => u.PreferredLocations)
                .ThenInclude(pl => pl.Province)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            var allProvinces = await _dbContext.Provinces.ToListAsync();
            var selectedProvinceCodes = user.PreferredLocations.Select(pl => pl.Province.Code).ToList();

            var model = new PersonalInfoViewModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                PreferredLocations = selectedProvinceCodes,

                Provinces = new MultiSelectList(allProvinces, "Code", "FullName", selectedProvinceCodes)
            };

            return PartialView("ManageCV/_PersonalInfoForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SubmitPersonalInfo")]
        public async Task<IActionResult> SubmitPersonalInfo(PersonalInfoViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                // Load lại danh sách provinces nếu model có lỗi
                var allProvinces = await _dbContext.Provinces.ToListAsync();
                model.Provinces = new MultiSelectList(allProvinces, "Code", "FullName", model.PreferredLocations);
                return PartialView("ManageCV/_PersonalInfoForm", model);
            }

            // ✅ Cập nhật tên và SĐT
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            // ✅ Cập nhật danh sách PreferredLocations
            // Xoá những location cũ
            var oldLocations = _dbContext.PreferredLocations.Where(p => p.UserId == user.Id);
            _dbContext.PreferredLocations.RemoveRange(oldLocations);

            // Thêm mới theo lựa chọn hiện tại
            var newLocations = model.PreferredLocations.Select(code => new PreferredLocation
            {
                UserId = user.Id,
                Code = code
            });

            await _dbContext.PreferredLocations.AddRangeAsync(newLocations);
            await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Thông tin cá nhân đã được cập nhật." });
        }

        // ------------------------------------------------------------

        [HttpGet]
        [Route("profile-cv")]
        public async Task<IActionResult> ProfileCV()
        {

            var userId = _userManager.GetUserId(User);
            var user = await _dbContext.Users
                .Include(u => u.PreferredLocations)
                .ThenInclude(p => p.Province)
                .Include(u => u.UserIndustries)
                .ThenInclude(p => p.Industry)

                  .Include(u => u.CurrentLevel)

                      .Include(u => u.UserSkills)
                .ThenInclude(p => p.Skill)

                  .Include(u => u.WorkType)
                  .Include(u => u.Educations)
                  .Include(u => u.WorkExperiences)
                  .Include(u => u.Certificates)
                  .Include(u => u.Awards)
                  .Include(u => u.PersonalProjects)
                  .Include(u => u.UserIndustries)
                  .ThenInclude(ui => ui.Industry)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var model = new PersonalDetailsViewModel
            {
                FullName = user.FullName,
                Title = user.ShortName,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Gender = user.Gender,
                Province = user.ProvinceCode, // nếu bạn chưa có tỉnh lưu trong DB
                Address = user.Address,
                PersonalLink = user.PersonLink
            };

            ViewData["user"] = user;
            ViewData["personalDetailsViewModel"] = model;
            ViewData["aboutMeViewModel"] = new AboutMeViewModel() { AboutMe = user.AboutMe };

            //ViewData["educationViewModel"] = new EducationViewModel
            //{
            //    School = ủ.School,
            //    Major = latestEducation.Major,
            //    FromMonth = latestEducation.FromMonth,
            //    FromYear = latestEducation.FromYear,
            //    ToMonth = latestEducation.ToMonth,
            //    ToYear = latestEducation.ToYear,
            //    IsStudying = latestEducation.IsStudying,
            //    Details = latestEducation.Details
            //};
            return View();
        }
        private async Task LoadUserJobCounters(string userId)
        {
            ViewData["totalApplyPosts"] = await _dbContext.ApplyPosts.CountAsync(ap => ap.UserID == userId);
            ViewData["totalFavorites"] = await _dbContext.Favorites.CountAsync(f => f.UserID == userId);
            ViewData["totalViewedPosts"] = await _dbContext.PostViews
                .Where(v => v.UserId == userId)
                .Select(v => v.PostId)
                .Distinct()
                .CountAsync();

            ViewBag.FavoritePostIds = await _dbContext.Favorites
                .Where(f => f.UserID == userId)
                .Select(f => f.PostID)
                .ToListAsync();
        }


        [HttpGet]
        [Route("my-jobs/applied")]
        public async Task<IActionResult> MyJobsApplied(int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _dbContext.ApplyPosts
                .Where(ap => ap.UserID == userId)
                .Include(ap => ap.Post)
                    .ThenInclude(p => p.Company)
                .Include(ap => ap.Post)
                    .ThenInclude(p => p.PostLocations)
                        .ThenInclude(pl => pl.Location)
                        .ThenInclude(l => l.Address)
                        .ThenInclude(a => a.City)
                .Select(ap => ap.Post)
                .Where(p => !p.IsDeleted);

            var totalItems = await query.CountAsync();

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Tab = "applied";
            ViewBag.Message = "Your applied jobs are stored for the last 12 months.";
            await LoadUserJobCounters(userId);

            return View("MyJobs", posts);
        }

        [HttpGet]
        [Route("my-jobs")]
        public async Task<IActionResult> MyJobs(int page = 1, int pageSize = 10, string sort_by = "latest_posting")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _dbContext.Favorites
                .Where(f => f.UserID == userId)
                .Include(f => f.Post)
                    .ThenInclude(p => p.Company)
                .Include(f => f.Post)
                    .ThenInclude(p => p.PostLocations)
                        .ThenInclude(pl => pl.Location)
                        .ThenInclude(l => l.Address)
                        .ThenInclude(a => a.City)
                .Select(f => f.Post)
                .Where(p => !p.IsDeleted);

            // Áp dụng sắp xếp
            query = sort_by switch
            {
                "about_to_expire" => query.OrderBy(p => p.Expired),
                "most_recent_viewed" => query.OrderByDescending(p => p.ViewTotal), // ví dụ
                _ => query.OrderByDescending(p => p.CreatedAt) // latest_posting
            };

            var totalItems = await query.CountAsync();
            var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentSort = sort_by;
            ViewBag.Tab = "saved";
            ViewBag.Message = "You can save up to 20 jobs.";
            await LoadUserJobCounters(userId);

            return View("MyJobs", posts);
        }


        [HttpGet]
        [Route("my-jobs/recent-viewed")]
        public async Task<IActionResult> MyJobsRecentViewed(int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Bước 1: Lấy danh sách PostId đã xem gần đây, loại trùng
            var recentPostIds = await _dbContext.PostViews
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.ViewedAt)
                .Select(v => v.PostId)
                .Distinct()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Bước 2: Truy vấn lại Post theo danh sách ID (bao gồm thông tin liên quan)
            var recentPosts = await _dbContext.Posts
                .Where(p => recentPostIds.Contains(p.PostId) && !p.IsDeleted)
                .Include(p => p.Company)
                .Include(p => p.PostLocations)
                    .ThenInclude(pl => pl.Location)
                    .ThenInclude(l => l.Address)
                    .ThenInclude(a => a.City)
                .ToListAsync();

            // Đảm bảo thứ tự bài viết khớp với ViewedAt (dựa theo thứ tự ID trong list)
            recentPosts = recentPosts
                .OrderBy(p => recentPostIds.IndexOf(p.PostId))
                .ToList();

            // Bước 3: Tổng số bài đã xem (để tính số trang)
            var totalViewedPosts = await _dbContext.PostViews
                .Where(v => v.UserId == userId)
                .Select(v => v.PostId)
                .Distinct()
                .CountAsync();

            // Gán thông tin phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalViewedPosts / (double)pageSize);

            // Bước 4: Counter cho tab
            await LoadUserJobCounters(userId);

            ViewBag.Message = "Only display jobs viewed in the last 3 months.";
            // Trả về view chung
            return View("MyJobs", recentPosts);
        }


        [HttpGet]
        [Route("job-invitation")]
        public IActionResult JobInvitation()
        {
            return View();
        }

        [HttpGet]
        [Route("email-subcription")]
        public IActionResult ESubscriptions()
        {
            return View();
        }
        [HttpGet]
        [Route("settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpGet]
        [Route("view-pdf/{filename?}")]
        public async Task<IActionResult> ViewPDF(string? filename)
        {
            var userId = _userManager.GetUserId(User);

            ResumeFile? resume = null;

            if (!string.IsNullOrEmpty(filename))
            {
                resume = await _dbContext.ResumeFiles
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.FileName == filename);
            }

            // Nếu không tìm thấy theo filename → lấy file mặc định
            if (resume == null)
            {
                resume = await _dbContext.ResumeFiles
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.IsPrimary);
            }

            if (resume == null)
            {
                return NotFound("Không tìm thấy file resume.");
            }

            ViewData["resume"] = resume;
            return View();
        }

        [HttpPost]
        [Route("UploadCv")]
        public async Task<IActionResult> UploadCv(ResumeUploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);
            var currentCount = await _dbContext.ResumeFiles.CountAsync(r => r.UserId == userId);
            if (currentCount >= 5)
            {
                ModelState.AddModelError("", "Bạn chỉ được upload tối đa 5 file CV.");
                return RedirectToAction("ManageCV");
            }

            var file = model.CvFile;

            if (file == null || file.Length == 0)
            {
                model.Message = "File không hợp lệ.";
                model.IsSuccess = false;
                return View(model);
            }

            if (file.Length > 3 * 1024 * 1024)
            {
                model.Message = "Dung lượng vượt quá 3MB.";
                model.IsSuccess = false;
                return View(model);
            }

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
            {
                model.Message = "Định dạng không hợp lệ. Chỉ chấp nhận .pdf, .doc, .docx";
                model.IsSuccess = false;
                return View(model);
            }

            var uuid = Guid.NewGuid().ToString();
            var originalFileName = file.FileName;
            string finalFileName = uuid + ".pdf"; // luôn lưu dưới dạng .pdf
            var relativePath = $"/resumes/{finalFileName}";
            string physicalPdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes", finalFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(physicalPdfPath)!);

            // Nếu file là .pdf → lưu thẳng
            if (ext == ".pdf")
            {
                using (var stream = new FileStream(physicalPdfPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            else
            {
                // Convert .doc/.docx → .pdf
                var tempPath = Path.Combine(Path.GetTempPath(), uuid + ext);
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Chuyển sang PDF
                var document = new Spire.Doc.Document();
                document.LoadFromFile(tempPath);
                document.SaveToFile(physicalPdfPath, Spire.Doc.FileFormat.PDF);

                // Xoá file gốc nếu cần
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
            }

            // Unmark những resume trước đó
            var resumes = _dbContext.ResumeFiles
                .Where(r => r.UserId == userId && r.IsPrimary);

            foreach (var r in resumes)
            {
                r.IsPrimary = false;
                r.UpdatedAt = DateTime.UtcNow;
            }

            var resume = new ResumeFile
            {
                FileName = finalFileName,
                OrigialFileName = originalFileName,
                FilePath = relativePath,
                UserId = userId,
                IsPrimary = true
            };

            _dbContext.ResumeFiles.Add(resume);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ManageCV");
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var resume = await _dbContext.ResumeFiles
                .FirstOrDefaultAsync(r => r.ResumeFileId == id && r.UserId == userId);

            if (resume == null || resume.IsPrimary)
                return RedirectToAction("ManageCV");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", resume.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.ResumeFiles.Remove(resume);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ManageCV");
        }

        [HttpPost]
        [Route("set-default")]
        public async Task<IActionResult> SetPrimary(int id)
        {
            var userId = _userManager.GetUserId(User);
            var resume = await _dbContext.ResumeFiles
                .FirstOrDefaultAsync(r => r.ResumeFileId == id && r.UserId == userId);

            if (resume == null)
                return RedirectToAction("ManageCV");

            // Reset tất cả về false
            var all = _dbContext.ResumeFiles.Where(r => r.UserId == userId);
            foreach (var r in all)
                r.IsPrimary = false;

            resume.IsPrimary = true;
            resume.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("ManageCV");
        }

        // ----------------------------  Personal Info  ----------------------------
        [HttpPost]
        [Route("SavePersonalInfo")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePersonalInfo(PersonalInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu lỗi, gán lại danh sách địa điểm (nếu bạn dùng dropdown động)
                //model.AvailableLocations = GetAvailableLocations();
                return View("ManageCV", model); // hoặc return Partial nếu là modal
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // ✅ Cập nhật thông tin
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            // ✅ Optional: Lưu preferred locations (nếu có bảng riêng)
            var existingPrefs = _dbContext.PreferredLocations.Where(p => p.UserId == userId);
            _dbContext.PreferredLocations.RemoveRange(existingPrefs);

            var newLocations = model.PreferredLocations
                .Select(loc => new PreferredLocation
                {
                    UserId = userId,
                    Code = loc
                });

            _dbContext.PreferredLocations.AddRange(newLocations);

            // ✅ Lưu thay đổi
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Thông tin cá nhân đã được cập nhật.";
            return RedirectToAction("ManageCV"); // hoặc trở về trang hồ sơ
        }
        // ---------------------------------------------------------------------



        [Route("GetPersonalDetailForm")]
        public async Task<IActionResult> GetPersonalDetailForm()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new PersonalDetailsViewModel
            {
                FullName = user.FullName,
                Title = user.ShortName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Province = user.ProvinceCode,
                Address = user.Address,
                PersonalLink = user.PersonalLink,
                AvatarUrl = user.Avatar // hoặc link xử lý URL ảnh
            };

            return PartialView("ProfileCV/_PersonalDetailForm", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SavePersonalDetail")]
        public async Task<IActionResult> SavePersonalDetail(PersonalDetailsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                model.AvatarUrl = user.Avatar;
                return PartialView("ProfileCV/_PersonalDetailForm", model);
            }

            // ✅ Cập nhật dữ liệu
            user.FullName = model.FullName;
            user.ShortName = model.Title;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.BirthDate = model.BirthDate;
            user.Gender = model.Gender;
            user.ProvinceCode = model.Province;
            user.Address = model.Address;
            user.PersonalLink = model.PersonalLink;

            // ✅ Upload avatar nếu có
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{model.AvatarFile.FileName}";
                var filePath = Path.Combine("wwwroot/images/avatars", fileName); // nhớ tạo thư mục này

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(stream);
                }

                user.Avatar = $"/images/avatars/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Không thể cập nhật thông tin. Vui lòng thử lại.");
                model.AvatarUrl = user.Avatar;
                return PartialView("ManageCV/_PersonalDetailForm", model);
            }

            return Json(new { success = true, message = "Thông tin cá nhân đã được lưu." });
        }
        // ----------------------------  About Me  ----------------------------

        [Route("GetAboutMe")]
        public IActionResult GetAboutMe()
        {
            return PartialView("ProfileCV/_AboutMeForm", new AboutMeViewModel());
        }

        [HttpPost]
        [Route("SaveAboutMe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAboutMe(AboutMeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProfileCV/_AboutMeForm", model);
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            user.AboutMe = model.AboutMe; // hoặc dùng field riêng nếu bạn muốn tách

            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        [Route("EditAboutMe")]
        public async Task<IActionResult> EditAboutMe()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            var viewModel = new AboutMeViewModel
            {
                AboutMe = user.AboutMe
            };

            return PartialView("ProfileCV/_AboutMeForm", viewModel);
        }

        [HttpPost]
        [Route("EditAboutMe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAboutMe(AboutMeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProfileCV/_AboutMeForm", model);
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            user.AboutMe = model.AboutMe;

            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        // ---------------------------------------------------------------------


        // --------------------------  Education  --------------------------
        [Route("GetEducationForm")]
        public IActionResult GetEducationForm()
        {
            return PartialView("ProfileCV/_EducationForm", new EducationViewModel());
        }

        [HttpPost]
        [Route("SaveEducation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEducation(EducationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProfileCV/_EducationForm", model);
            }
            if (!model.IsStudying && (!model.ToMonth.HasValue || !model.ToYear.HasValue))
            {
                ModelState.AddModelError("", "Vui lòng nhập thời gian kết thúc hoặc đánh dấu là đang học.");
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            var education = new Education
            {
                UserId = userId,
                School = model.School,
                Major = model.Major,
                IsStudying = model.IsStudying,
                FromMonth = model.FromMonth,
                FromYear = model.FromYear,
                ToMonth = model.IsStudying ? null : model.ToMonth,
                ToYear = model.IsStudying ? null : model.ToYear,
                Details = model.Details
            };

            _dbContext.Educations.Add(education);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Route("EditEducation")]
        public IActionResult GetEditEducation(int id)
        {
            var education = _dbContext.Educations.FirstOrDefault(e => e.Id == id);
            if (education == null) return NotFound();

            var viewModel = new EducationViewModel()
            {
                EducationId = education.Id,
                School = education.School,
                Major = education.Major,
                FromMonth = education.FromMonth,
                FromYear = education.FromYear,
                ToMonth = education.ToMonth,
                ToYear = education.ToYear,
                IsStudying = education.IsStudying,
                Details = education.Details
            };

            return PartialView("ProfileCV/_EducationForm", viewModel);
        }

        [HttpPost]
        [Route("EditEducation")]
        public IActionResult EditEducation(EducationViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("ProfileCV/_EducationForm", model);

            var edu = _dbContext.Educations.FirstOrDefault(e => e.Id == model.EducationId);
            if (edu != null)
            {
                edu.School = model.School;
                edu.Major = model.Major;
                edu.FromMonth = model.FromMonth;
                edu.FromYear = model.FromYear;
                edu.ToMonth = model.ToMonth;
                edu.ToYear = model.ToYear;
                edu.IsStudying = model.IsStudying;
                edu.Details = model.Details;

                _dbContext.SaveChanges();
            }

            return Json(new { success = true });
        }


        [HttpPost]
        [Route("DeleteEducation")]
        public IActionResult DeleteEducation(int EducationId)
        {
            var edu = _dbContext.Educations.FirstOrDefault(e => e.Id == EducationId);
            if (edu != null)
            {
                _dbContext.Educations.Remove(edu);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("ProfileCV");
        }

        // -----------------------------------------------------------------------

       


       


        // ----------------------------  Work Experience  ----------------------------

        [Route("GetWorkExperience")]
        public IActionResult GetWorkExperienceForm()
        {
            return PartialView("ProfileCV/_WorkExperienceForm", new WorkExperienceViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SaveWorkExperience")]
        public async Task<IActionResult> SaveWorkExperience(WorkExperienceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProfileCV/_WorkExperienceForm", model);
            }

            var userId = _userManager.GetUserId(User);

            var entity = new WorkExperience
            {
                UserId = userId,
                JobTitle = model.JobTitle,
                Company = model.Company,
                FromMonth = model.FromMonth,
                FromYear = model.FromYear,
                ToMonth = model.IsCurrentlyWorking ? null : model.ToMonth,
                ToYear = model.IsCurrentlyWorking ? null : model.ToYear,
                IsCurrentlyWorking = model.IsCurrentlyWorking,
                Description = model.Description,
                Project = model.Project
            };

            _dbContext.WorkExperiences.Add(entity);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Route("EditWorkExperience")]
        public IActionResult GetEditWorkExperience(int id)
        {
            var work = _dbContext.WorkExperiences.FirstOrDefault(w => w.Id == id);
            if (work == null) return NotFound();

            var viewModel = new WorkExperienceViewModel
            {
                Id = work.Id,
                JobTitle = work.JobTitle,
                Company = work.Company,
                FromMonth = work.FromMonth,
                FromYear = work.FromYear,
                ToMonth = work.ToMonth,
                ToYear = work.ToYear,
                IsCurrentlyWorking = work.IsCurrentlyWorking,
                Description = work.Description,
                Project = work.Project
            };

            return PartialView("ProfileCV/_WorkExperienceForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditWorkExperience")]
        public async Task<IActionResult> EditWorkExperience(WorkExperienceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProfileCV/_WorkExperienceForm", model);
            }

            var work = _dbContext.WorkExperiences.FirstOrDefault(w => w.Id == model.Id);
            if (work == null) return NotFound();

            work.JobTitle = model.JobTitle;
            work.Company = model.Company;
            work.FromMonth = model.FromMonth;
            work.FromYear = model.FromYear;
            work.ToMonth = model.IsCurrentlyWorking ? null : model.ToMonth;
            work.ToYear = model.IsCurrentlyWorking ? null : model.ToYear;
            work.IsCurrentlyWorking = model.IsCurrentlyWorking;
            work.Description = model.Description;
            work.Project = model.Project;

            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteWorkExperience")]
        public async Task<IActionResult> DeleteWorkExperience(int id)
        {
            var work = _dbContext.WorkExperiences.FirstOrDefault(w => w.Id == id);
            if (work != null)
            {
                _dbContext.WorkExperiences.Remove(work);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("ProfileCV");
        }

        // ---------------------------------------------------------------------

       


        // ----------------------------  Cover Letter  ----------------------------

        [HttpPost]
        [Route("SaveCoverLetter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCoverLetter(CoverLetterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); // hoặc trả về Partial nếu dùng modal

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            user.CoverLetter = model.CoverLetter;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Cập nhật Cover Letter thành công.";
            return RedirectToAction("ManageCV");
        }
        // ---------------------------------------------------------------------


        // ----------------------------  Personal Details  ---------------------
        [HttpPost]
        [Route("SavePersonalDetails")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePersonalDetails(PersonalDetailsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); // hoặc return PartialView nếu là modal

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.FullName = model.FullName;
            user.ShortName = model.Title;
            user.PhoneNumber = model.PhoneNumber;
            user.BirthDate = model.BirthDate;
            user.Gender = model.Gender;
            user.PersonLink = model.PersonalLink;
            user.AcademicLevel = user.AcademicLevel; // giữ nguyên nếu không có

            user.Address = model.Address;
            user.PersonLink = model.PersonalLink;

            // Handle Avatar Upload (nếu có)
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                var ext = Path.GetExtension(model.AvatarFile.FileName).ToLower();
                var fileName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(stream);
                }

                user.Avatar = "/uploads/avatars/" + fileName;
            }

            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Thông tin cá nhân đã được cập nhật.";
            return RedirectToAction("ManageCV");
        }
        // ---------------------------------------------------------------------


        // ----------------------------  Project  ----------------------------
        [HttpGet]
        [Route("GetProject")]
        public IActionResult GetProjectForm()
        {
            return PartialView("ProfileCV/_PersonalProjectForm", new PersonalProjectViewModel());
        }

        [HttpPost]
        [Route("SaveProject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProject(PersonalProjectViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("ProfileCV/_PersonalProjectForm", model);

            var userId = _userManager.GetUserId(User);

            var project = new PersonalProject
            {
                ProjectName = model.ProjectName,
                IsOngoing = model.IsOngoing,
                StartMonth = model.StartMonth,
                StartYear = model.StartYear,
                EndMonth = model.IsOngoing ? null : model.EndMonth,
                EndYear = model.IsOngoing ? null : model.EndYear,
                Description = model.Description,
                UrlProject = model.UrlProject,
                UserId = userId
            };

            _dbContext.PersonalProjects.Add(project);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        [Route("EditProject")]
        public IActionResult EditProject(int id)
        {
            var project = _dbContext.PersonalProjects.FirstOrDefault(p => p.Id == id);
            if (project == null) return NotFound();

            var viewModel = new PersonalProjectViewModel
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                IsOngoing = project.IsOngoing,
                StartMonth = project.StartMonth,
                StartYear = project.StartYear,
                EndMonth = project.EndMonth,
                EndYear = project.EndYear,
                Description = project.Description,
                UrlProject = project.UrlProject
            };

            return PartialView("ProfileCV/_PersonalProjectForm", viewModel);
        }

        [HttpPost]
        [Route("EditProject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProject(PersonalProjectViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("ProfileCV/_PersonalProjectForm", model);

            var project = _dbContext.PersonalProjects.FirstOrDefault(p => p.Id == model.Id);
            if (project == null) return NotFound();

            project.ProjectName = model.ProjectName;
            project.IsOngoing = model.IsOngoing;
            project.StartMonth = model.StartMonth;
            project.StartYear = model.StartYear;
            project.EndMonth = model.IsOngoing ? null : model.EndMonth;
            project.EndYear = model.IsOngoing ? null : model.EndYear;
            project.Description = model.Description;
            project.UrlProject = model.UrlProject;

            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeletePersonalProject")]
        public async Task<IActionResult> DeletePersonalProject(int id)
        {
            var project = _dbContext.PersonalProjects.FirstOrDefault(a => a.Id == id);
            if (project != null)
            {
                _dbContext.PersonalProjects.Remove(project);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("ProfileCV");
        }
        // ---------------------------------------------------------------------


        // ----------------------------  Awards  ----------------------------
        // GET: Mở form thêm giải thưởng
        [Route("GetAward")]
        public IActionResult GetAwardForm()
        {
            return PartialView("ProfileCV/_AwardsForm", new AwardViewModel());
        }

        // POST: Lưu giải thưởng mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SaveAward")]
        public async Task<IActionResult> SaveAward(AwardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProfileCV/_AwardsForm", model);
            }

            var userId = _userManager.GetUserId(User);

            var award = new Award
            {
                UserId = userId,
                Name = model.Name,
                Organization = model.Organization,
                IssueMonth = model.IssueMonth,
                IssueYear = model.IssueYear,
                Description = model.Description
            };

            _dbContext.Awards.Add(award);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: Mở form sửa giải thưởng
        [Route("EditAward")]
        public IActionResult GetEditAward(int id)
        {
            var award = _dbContext.Awards.FirstOrDefault(a => a.Id == id);
            if (award == null) return NotFound();

            var viewModel = new AwardViewModel
            {
                Name = award.Name,
                Organization = award.Organization,
                IssueMonth = award.IssueMonth,
                IssueYear = award.IssueYear,
                Description = award.Description
            };

            return PartialView("ProfileCV/_AwardsForm", viewModel);
        }

        // POST: Cập nhật giải thưởng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditAward")]
        public async Task<IActionResult> EditAward(int id, AwardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProfileCV/_AwardsForm", model);
            }

            var award = _dbContext.Awards.FirstOrDefault(a => a.Id == id);
            if (award == null) return NotFound();

            award.Name = model.Name;
            award.Organization = model.Organization;
            award.IssueMonth = model.IssueMonth;
            award.IssueYear = model.IssueYear;
            award.Description = model.Description;

            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: Xoá giải thưởng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteAward")]
        public async Task<IActionResult> DeleteAward(int id)
        {
            var award = _dbContext.Awards.FirstOrDefault(a => a.Id == id);
            if (award != null)
            {
                _dbContext.Awards.Remove(award);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("ProfileCV");
        }

        // ---------------------------------------------------------------------
        // ----------------------------  General Info  ----------------------------
        [HttpPost]
        [Route("SaveGeneralInfo")]
        public async Task<IActionResult> SaveGeneralInfo(GeneralInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.Users
                .Include(u => u.UserIndustries)
                .FirstOrDefaultAsync(u => u.Id == userId);

            // Cập nhật basic fields
            user.YearsOfExperience = model.YearsOfExperience;
            user.LevelId = model.LevelId;
            user.WorkTypeId = model.WorkTypeId;
            user.MinExpectedSalary = model.MinExpectedSalary;
            user.MaxExpectedSalary = model.MaxExpectedSalary;
            user.CurrentSalary = model.CurrentSalary;

            // Xóa các ngành cũ
            _dbContext.UserIndustries.RemoveRange(user.UserIndustries);

            // Thêm mới ngành đã chọn
            var newIndustries = model.SelectedIndustryIds.Select(id => new UserIndustry
            {
                UserId = userId,
                IndustryId = id
            });

            await _dbContext.UserIndustries.AddRangeAsync(newIndustries);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ManageCV");
        }
        // ---------------------------------------------------------------------

        // ----------------------------  SKills  ----------------------------

        [HttpGet]
        [Route("GetSkills")]
        public IActionResult GetSkillForm()
        {
            var userId = _userManager.GetUserId(User);

            var user = _dbContext.Users
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null) return NotFound();

            // Group kỹ năng
            var groupedSkillsDict = user.UserSkills
                .GroupBy(s => s.SkillLevel)
                .ToDictionary(g => g.Key, g => g.ToList());

            var levels = new List<string> { "Beginner", "Intermediate", "Excellent" };

            var groupedSkills = levels.Select(level => new GroupedUserSkill
            {
                SkillLevel = level,
                Skills = groupedSkillsDict.ContainsKey(level) ? groupedSkillsDict[level] : new List<UserSkill>()
            }).ToList();
            var skillLevels = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Beginner", Value = "Beginner" },
                    new SelectListItem { Text = "Intermediate", Value = "Intermediate" },
                    new SelectListItem { Text = "Excellent", Value = "Excellent" }
                };
            var skillItems = _dbContext.Skills
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.SkillId.ToString()
                }).ToList();

            var viewModel = new UserSkillsViewModel
            {
                GroupedSkills = groupedSkills,
                SkillLevelOptions = skillLevels,
                SkillOptions = skillItems
            };

            return PartialView("ProfileCV/_SkillsForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SaveSkills")]
        public async Task<IActionResult> SaveSkills()
        {
            var userId = _userManager.GetUserId(User);

            var addedSkillNames = Request.Form["profile[skills][][name]"];
            var addedSkillLevels = Request.Form["profile[skills][][level]"];
            var deletedSkillNames = Request.Form["profile[skills_delete][]"];

            // 1. Xóa các kỹ năng theo yêu cầu
            if (deletedSkillNames.Any())
            {
                var skillsToDelete = _dbContext.UserSkills
                    .Where(us => us.UserId == userId && deletedSkillNames.Contains(us.Skill.Name));

                _dbContext.UserSkills.RemoveRange(skillsToDelete);
            }

            // 2. Thêm các kỹ năng mới (nếu chưa tồn tại)
            for (int i = 0; i < addedSkillNames.Count; i++)
            {
                var name = addedSkillNames[i];
                var level = addedSkillLevels[i];

                var skill = _dbContext.Skills.FirstOrDefault(s => s.Name == name);
                if (skill == null) continue;

                bool alreadyExists = _dbContext.UserSkills
                    .Any(us => us.UserId == userId && us.SkillId == skill.SkillId);

                if (!alreadyExists)
                {
                    var userSkill = new UserSkill
                    {
                        UserId = userId,
                        SkillId = skill.SkillId,
                        SkillLevel = level
                    };
                    _dbContext.UserSkills.Add(userSkill);
                }
            }

            await _dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ---------------------------------------------------------------------

        // ----------------------------  Certificate  ----------------------------

        [Route("GetCertificateForm")]
        public IActionResult GetCertificateForm()
        {
            return PartialView("ProfileCV/_CertificatesForm", new CertificateViewModel());
        }

        [Route("EditCertificate")]
        public IActionResult GetEditCertificate(int id)
        {
            var certificate = _dbContext.Certificates.FirstOrDefault(c => c.Id == id);
            if (certificate == null) return NotFound();

            var viewModel = new CertificateViewModel
            {
                Name = certificate.Name,
                Organization = certificate.Organization,
                IssueMonth = certificate.IssueMonth,
                IssueYear = certificate.IssueYear,
                Url = certificate.Url,
                Description = certificate.Description
            };

            return PartialView("ProfileCV/_CertificatesForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SaveCertificate")]
        public async Task<IActionResult> SaveCertificate(CertificateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu form không hợp lệ, return lại partial view để hiện lỗi
                return PartialView("ProfileCV/_CertificatesForm", model);
            }
            var userId = _userManager.GetUserId(User);
            // Tạo một entity mới từ dữ liệu view model
            var certificate = new Certificate
            {
                UserId = userId,
                Name = model.Name,
                Organization = model.Organization,
                IssueMonth = model.IssueMonth,
                IssueYear = model.IssueYear,
                Url = model.Url,
                Description = model.Description
            };

            // Thêm vào context và lưu
            _dbContext.Certificates.Add(certificate);
            await _dbContext.SaveChangesAsync();

            // Trả về kết quả thành công (có thể dùng Redirect hoặc Json tùy theo client xử lý)
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditCertificate")]
        public async Task<IActionResult> EditCertificate(int id, CertificateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("ProfileCV/_CertificatesForm", model);
            }

            var certificate = _dbContext.Certificates.FirstOrDefault(c => c.Id == id);
            if (certificate == null) return NotFound();

            certificate.Name = model.Name;
            certificate.Organization = model.Organization;
            certificate.IssueMonth = model.IssueMonth;
            certificate.IssueYear = model.IssueYear;
            certificate.Url = model.Url;
            certificate.Description = model.Description;

            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteCertificate")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = _dbContext.Certificates.FirstOrDefault(c => c.Id == id);
            if (certificate == null) return NotFound();

            _dbContext.Certificates.Remove(certificate);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ProfileCV"); // hoặc return Json({ success = true }) nếu dùng ajax
        }
    }
}
