using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RecruitmentApp.Areas.User.Jobs.ModelViews;
using RecruitmentApp.DTOs;
using RecruitmentApp.Models;
using RecruitmentApp.ModelViews;
using RecruitmentApp.Services.Users;
using RecruitmentApp.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RecruitmentApp.Areas.User.Jobs.Controllers
{
    [Area("User/Jobs")]
    [Route("/jobs")]
    public class JobController : Controller
    {

        private readonly AppDbContext _dbContext;
        private readonly PostService _postService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public JobController(AppDbContext context, PostService postService, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _dbContext = context;
            _postService = postService;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            var industries = _dbContext.Industries.ToList();
            var posts = _dbContext.Posts
              .Include(p => p.PostSkills)
              .ThenInclude(pk => pk.Skill)
              .Include(p => p.Company)

              .Include(p => p.Location)
              .ThenInclude(pl => pl.Address)
              .ThenInclude(pla => pla.City)

              .ToList();


            ViewData["industries"] = industries;
            return View();
        }
        [HttpGet]
        [Route("{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var post = _dbContext.Posts
                .Include(p => p.Company)
                .ThenInclude(c => c.Images)

                .Include(p => p.Company)
                .ThenInclude(c => c.CompanyIndustries)
                    .ThenInclude(ci => ci.Industry)

                .Include(p => p.Company)
                    .ThenInclude(c => c.Country)

                .Include(p => p.PostSkills)
                    .ThenInclude(ps => ps.Skill)

                .Include(p => p.PostLocations)
                    .ThenInclude(pl => pl.Location)
                        .ThenInclude(l => l.Address)
                .Include(f => f.Favorites)

                .SingleOrDefault(p => p.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            // Lấy danh sách kỹ năng để tìm bài viết tương tự
            var postSkillIds = post.PostSkills.Select(ps => ps.Skill.SkillId).ToList();

            var suggestedPosts = _dbContext.Posts
                .Include(p => p.PostSkills)
                    .ThenInclude(ps => ps.Skill)
                .Include(p => p.Company)
               
                .Include(p => p.PostLocations)
                    .ThenInclude(pl => pl.Location)
                        .ThenInclude(l => l.Address)
                .Where(p => p.PostSkills.Any(ps => postSkillIds.Contains(ps.SkillID)) && p.PostId != post.PostId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToList();

            // Ghi nhận lượt xem
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            bool hasViewed = await _dbContext.PostViews.AnyAsync(v =>
                v.PostId == post.PostId &&
                ((userId != null && v.UserId == userId) || (userId == null && v.IpAddress == ipAddress))
            );

            if (!hasViewed)
            {
                var view = new PostView
                {
                    PostId = post.PostId,
                    UserId = userId,
                    IpAddress = ipAddress,
                    ViewedAt = DateTime.Now
                };

                _dbContext.PostViews.Add(view);
                await _dbContext.SaveChangesAsync(); // <== thiếu SaveChanges ở bản trước
            }

            var isFavorited = post.Favorites?.Any(f => f.UserID == userId);

            ViewData["isFavorited"] = isFavorited;
            ViewData["post"] = post;
            ViewData["suggestedPosts"] = suggestedPosts;

            return View();
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string province = "0", string key = "all", int page = 1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int pageSize = 10; // Số lượng bài đăng trên mỗi trang

            var industries = _dbContext.Industries.ToList();

            var query = _dbContext.Posts
                .Include(p => p.PostSkills)
                .ThenInclude(pk => pk.Skill)

                .Include(p => p.Company)
                .ThenInclude(cc => cc.Country)

                .Include(p => p.Company)
                .ThenInclude(cc => cc.CompanyIndustries)
                .ThenInclude(ci => ci.Industry)


                .Include(p => p.PostLocations)
                .ThenInclude(pl => pl.Location)
                .ThenInclude(pla => pla.Address)
                .ThenInclude(pla => pla.City)

                .OrderByDescending(p => p.PostDate)

                .AsQueryable();

            // Chỉ lọc nếu key KHÔNG phải "all"
            if (!string.IsNullOrEmpty(key) && key != "all")
            {
                query = query.Where(p =>
                    p.Title.Contains(key) ||
                    p.Description.Contains(key) ||
                    p.Company.Name.Contains(key) ||
                    p.PostSkills.Any(ps => ps.Skill.Name.Contains(key))
                );
            }

            int totalPosts = query.Count(); // Tổng số bài đăng tìm được
            var posts = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var postDTOs = posts.Select(p => new PostDTO
            {
                PostId = p.PostId,
                Title = p.Title,
                Slug = p.Slug,
                IsHot = p.IsHot,
                ViewTotal = p.ViewTotal,
                Salary = p.Salary,
                SalaryText = p.salaryToString(),
                PostDate = p.PostDate,
                Expired = p.Expired,
                WorkSpace = p.WorkSpace,
                TimeAgo = TimeFormatter.GetTimeAgo(p.PostDate),

                Description = p.Description,
                Benefit = p.Benefit,
                DegreeRequirement = p.DegreeRequirement,
                JobRequirement = p.JobRequirement,
                TopReason = p.TopReason,
                Company = p.Company != null ? new CompanyDTO
                {
                    CompanyId = p.Company.CompanyId,
                    Name = p.Company.Name,
                    Slug = p.Company.Slug,
                    Size = p.Company.Size,
                    Description = p.Company.Description,
                    Phone = p.Company.Phone,
                    Email = p.Company.Email,
                    Type = p.Company.Type,
                    WorkingTime = p.Company.WorkingTime,
                    LogoImage = p.Company.LogoImage,
                    Country = p.Company.Country.Name,
                    CountryCode = p.Company.Country.ISOCode,
                    ShortDescription = p.Company.ShortDescription,
                    CompanyUrl = p.Company.CompanyUrl,
                    Skills = p.Company.CompanySkills?.Select(cs => cs.Skill.Name).ToList(),
                    Locations = p.Company.Locations?.Select(l => l.Address.City.Name).ToList(),
                    Industries = p.Company.CompanyIndustries.Select(ps => new IndustryDTO
                    {
                        IndustryId = ps.Industry.IndustryId,
                        Name = ps.Industry.Name,
                    }).ToList(),
                } : null,
                Skills = p.PostSkills.Select(ps => new SkillDTO
                {
                    SkillId = ps.Skill.SkillId,
                    Name = ps.Skill.Name,
                    Slug = ps.Skill.Slug
                }).ToList(),

                Addresses = p.PostLocations != null ? p.PostLocations.Select(pl => new AddressDTO
                {
                    AddressId = pl.Location.Address.AddressId,
                    Nation = pl.Location.Address.City?.Name,
                    ProvinceCode = pl.Location.Address.City?.CodeName,
                    Province = pl.Location.Address.City?.FullName,
                    DetailPosition = pl.Location.Address.DetailPosition,
                    GgMapSrc = pl.Location.Address.GgMapSrc,
                    FullAddress = pl.Location.Address.FullAddress
                }).ToList() : new List<AddressDTO>()

            }).ToList();
            ViewData["industries"] = industries;

            var filterData = new FilterDataComponent() {
                    post = postDTOs.FirstOrDefault(),
                    posts = postDTOs,
                    currentPage = page,
                    totalPages = (int)Math.Ceiling((double)totalPosts / pageSize),
                    totalPosts = totalPosts,
                    key = key,
                    province = province,
            };

            ViewData["filterData"] = filterData;

            ViewData["posts"] = postDTOs;
            ViewData["currentPage"] = page;
            ViewData["totalPages"] = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewData["province"] = province;
            ViewData["totalPosts"] = totalPosts;
            ViewData["key"] = key;

            ViewData["post"] = postDTOs.FirstOrDefault();

            ViewData["Title"] = key == "all" ? "Việc làm IT, tuyển dụng IT tại Việt Nam" : $"Tuyển dụng {key}, việc làm IT chất nhất tại Việt Nam";


            ViewData["filterViewModel"] = new FilterViewModel
            {
                Industries = industries,
                Levels = _dbContext.Levels.ToList(),
                WorkTypes = _dbContext.WorkTypes.ToList(),
                CompanyTypes = _dbContext.CompanyTypes.ToList()
            };

            return View();
        }

        [HttpGet]
        [Route("detail")]
        public IActionResult GetJobDetail(int postId)
        {
            var postDTOs = _postService.GetById(postId);

            if (postDTOs == null)
            {
                return NotFound();
            }
            return ViewComponent("JobDetail", new { post = postDTOs });
        }

        [HttpPost]
        [Route("filter")]
        public IActionResult GetJobSearch([FromBody] FilterModelView filter)
        {
            var key = filter.key;

            int pageSize = 10; // Số lượng bài đăng trên mỗi trang

            var industries = _dbContext.Industries.ToList();

            var query = _dbContext.Posts
                .Include(p => p.PostSkills)
                .ThenInclude(pk => pk.Skill)

                .Include(p => p.Company)
                .ThenInclude(cc => cc.Country)  

                .Include(p => p.Company)
                .ThenInclude(cc => cc.CompanyIndustries)
                .ThenInclude(ci => ci.Industry)


                .Include(p => p.PostLocations)
                .ThenInclude(pl => pl.Location)
                .ThenInclude(pla => pla.Address)
                .ThenInclude(pla => pla.City)

                .Include(p => p.PostLevels)
                .ThenInclude(pl => pl.Level)

                .OrderByDescending(p => p.PostDate)

                .AsQueryable();


            

            // Chỉ lọc nếu key KHÔNG phải "all"
            if (!string.IsNullOrEmpty(key) && key != "all")
            {
                query = query.Where(p =>
                    p.Title.Contains(key) ||
                    p.Description.Contains(key) ||
                    p.Company.Name.Contains(key) ||
                    p.PostSkills.Any(ps => ps.Skill.Name.Contains(key))
                );
            }


            // 🔹 **Lọc theo cấp bậc công việc (jobLevels)**
            if (filter.jobLevels != null && filter.jobLevels.Any())
            {
                query = query.Where(p => p.PostLevels.Any(pl => filter.jobLevels.Contains(pl.Level.Name)));
            }

            // 🔹 **Lọc theo mô hình làm việc (workingModels)**
            if (filter.workingModels != null && filter.workingModels.Any())
            {
                query = query.Where(p => filter.workingModels.Contains(p.WorkSpace));
            }

            // 🔹 **Lọc theo ngành (industries)**
            if (filter.industries != null && filter.industries.Any())
            {
                query = query.Where(p => p.Company.CompanyIndustries.Any(ci => filter.industries.Contains(ci.Industry.Name)));
            }

            // 🔹 **Lọc theo loại công ty (companyTypes)**
            if (filter.companyTypes != null && filter.companyTypes.Any())
            {
                query = query.Where(p => filter.companyTypes.Contains(p.Company.Type));
            }

            // 🔹 **Lọc theo tỉnh/thành phố (province)**
            if (!string.IsNullOrEmpty(filter.province) && filter.province != "all")
            {
                query = query.Where(p => p.PostLocations.Any(pl => pl.Location.Address.City.CodeName == filter.province));
            }

            // 🔹 **Lọc theo mức lương (salary)**
            //if (filter.salary != null)
            //{
            //    query = query.Where(p => p.MinSalary >= filter.salary.min && p.MaxSalary <= filter.salary.max);
            //}

            int totalPosts = query.Count(); // Tổng số bài đăng tìm được
            var posts = query.Skip((filter.page - 1) * pageSize).Take(pageSize).ToList();

            var postDTOs = posts.Select(p => new PostDTO
            {
                PostId = p.PostId,
                Title = p.Title,
                Slug = p.Slug,
                IsHot = p.IsHot,
                ViewTotal = p.ViewTotal,
                Salary = p.Salary,
                SalaryText = p.salaryToString(),
                PostDate = p.PostDate,
                Expired = p.Expired,
                WorkSpace = p.WorkSpace,
                TimeAgo = TimeFormatter.GetTimeAgo(p.PostDate),

                Description = p.Description,
                Benefit = p.Benefit,
                DegreeRequirement = p.DegreeRequirement,
                JobRequirement = p.JobRequirement,
                TopReason = p.TopReason,
                Company = p.Company != null ? new CompanyDTO
                {
                    CompanyId = p.Company.CompanyId,
                    Name = p.Company.Name,
                    Slug = p.Company.Slug,
                    Size = p.Company.Size,
                    Description = p.Company.Description,
                    Phone = p.Company.Phone,
                    Email = p.Company.Email,
                    Type = p.Company.Type,
                    WorkingTime = p.Company.WorkingTime,
                    LogoImage = p.Company.LogoImage,
                    Country = p.Company.Country.Name,
                    CountryCode = p.Company.Country.ISOCode,
                    ShortDescription = p.Company.ShortDescription,
                    CompanyUrl = p.Company.CompanyUrl,
                    Skills = p.Company.CompanySkills?.Select(cs => cs.Skill.Name).ToList(),
                    Locations = p.Company.Locations?.Select(l => l.Address.City.Name).ToList(),
                    Industries = p.Company.CompanyIndustries.Select(ps => new IndustryDTO
                    {
                        IndustryId = ps.Industry.IndustryId,
                        Name = ps.Industry.Name,
                    }).ToList(),
                } : null,
                Skills = p.PostSkills.Select(ps => new SkillDTO
                {
                    SkillId = ps.Skill.SkillId,
                    Name = ps.Skill.Name,
                    Slug = ps.Skill.Slug
                }).ToList(),

                Addresses = p.PostLocations != null ? p.PostLocations.Select(pl => new AddressDTO
                {
                    AddressId = pl.Location.Address.AddressId,
                    Nation = pl.Location.Address.City?.Name,
                    ProvinceCode = pl.Location.Address.City?.CodeName,
                    Province = pl.Location.Address.City?.FullName,
                    DetailPosition = pl.Location.Address.DetailPosition,
                    GgMapSrc = pl.Location.Address.GgMapSrc,
                    FullAddress = pl.Location.Address.FullAddress
                }).ToList() : new List<AddressDTO>()

            }).ToList();

            var filterData = new FilterDataComponent()
            {
                post = postDTOs.FirstOrDefault(),
                posts = postDTOs,
                currentPage = filter.page,
                totalPages = (int)Math.Ceiling((double)totalPosts / pageSize),
                totalPosts = totalPosts,
                key = key,
                province = filter.province,
            };
            return ViewComponent("Filter", new { filterData = filterData });
        }


        [HttpGet]
        [Route("apply/{slug}")]
        public async Task<IActionResult> Apply(string slug)
        {
            var userId = _userManager.GetUserId(User);
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            var primaryResume = await _dbContext.ResumeFiles
                .Where(r => r.UserId == userId && r.IsPrimary)
                .FirstOrDefaultAsync();

            var model = new ApplyPostViewModel
            {
                PostId = post.PostId,
                slugPost = post.Slug
            };

            if (primaryResume != null)
            {
                model.PrimaryResumeId = primaryResume.ResumeFileId;
                model.PrimaryResumeName = primaryResume.OrigialFileName;
                model.PrimaryResumePath = primaryResume.FilePath;
            }

            return View(model);
        }

        [HttpPost]
        [Route("apply/{slug}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(string slug, ApplyPostViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string fileName = null, originFileName = null, filePath = null;

            // Nếu người dùng upload file mới
            if (model.UploadedFile != null && model.UploadedFile.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads", "cvs");
                Directory.CreateDirectory(folder);
                fileName = Guid.NewGuid() + Path.GetExtension(model.UploadedFile.FileName);
                filePath = Path.Combine("/uploads/cvs", fileName);

                using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(stream);
                }

                originFileName = model.UploadedFile.FileName;
            }
            // Nếu người dùng dùng CV mặc định
            else if (model.PrimaryResumeId.HasValue)
            {
                var resume = await _dbContext.ResumeFiles
                    .FirstOrDefaultAsync(r => r.ResumeFileId == model.PrimaryResumeId && r.UserId == userId);

                if (resume != null)
                {
                    fileName = resume.FileName;
                    originFileName = resume.OrigialFileName;
                    filePath = resume.FilePath;
                }
            }

            var apply = new ApplyPost
            {
                PostID = model.PostId,
                UserID = userId,
                Name = model.Name,
                Phone = model.Phone,
                Description = model.Description,
                FileName = fileName,
                OriginFileName = originFileName,
                FilePath = filePath,
                ApplyDate = DateTime.Now
            };

            _dbContext.ApplyPosts.Add(apply);
            await _dbContext.SaveChangesAsync();

            TempData["Message"] = "Ứng tuyển thành công!";
            return RedirectToAction("Detail", "Job", new { slug = model.slugPost });
        }

        [HttpPost]
        [Route("Favorite")]
        public async Task<IActionResult> Favorite(int postId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var post = await _dbContext.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            var exists = await _dbContext.Favorites
                .AnyAsync(f => f.PostID == postId && f.UserID == userId);

            if (!exists)
            {
                _dbContext.Favorites.Add(new Favorite
                {
                    PostID = postId,
                    UserID = userId
                });

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Detail", "Job", new { slug = post.Slug });
        }

        [HttpPost]
        [Route("RemoveFavorite")]
        public async Task<IActionResult> RemoveFavorite(int postId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var post = await _dbContext.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            var favorite = await _dbContext.Favorites
                .FirstOrDefaultAsync(f => f.PostID == postId && f.UserID == userId);

            if (favorite != null)
            {
                _dbContext.Favorites.Remove(favorite);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Detail", "Job", new { slug = post.Slug });
        }

    }
}
