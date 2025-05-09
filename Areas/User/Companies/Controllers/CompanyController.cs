﻿using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;
using RecruitmentApp.Services;

namespace RecruitmentApp.Areas.User.Companies.Controllers
{
    [Area("User/Companies")]

    [Route("/companies")]
    public class CompanyController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly UserService _userService;
        private readonly ReviewStatsService _reviewStatsService;
        public CompanyController(AppDbContext context, UserManager<AppUser> userManager, UserService userService, ReviewStatsService reviewStatsService)
        {
            _dbContext = context;
            _userManager = userManager;
            _userService = userService;
            _reviewStatsService = reviewStatsService;
            _reviewStatsService = reviewStatsService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var list = await _dbContext.Companies
                .Include(c => c.Images)
                .Include(c => c.Posts)
                .Include(c => c.Locations)
                    .ThenInclude(cl => cl.Address)
                        .ThenInclude(a => a.City)
                .Include(c => c.Reviews)
                .ToListAsync();
            return View(list);
        }


        [HttpGet]
        [Route("{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var company = _dbContext.Companies
                .Include(c => c.Posts)
                .Include(c => c.Reviews)
                .Include(c => c.CompanyType)
                .Include(c => c.Locations)
                    .ThenInclude(cl => cl.Address)
                        .ThenInclude(a => a.City)

                .Include(c => c.Posts)
                    .ThenInclude(p => p.PostLevels)
                        .ThenInclude(l => l.Level)

                 .Include(c => c.Posts)
                    .ThenInclude(p => p.PostWorkTypes)
                        .ThenInclude(l => l.WorkType)
                .Include(c => c.Posts)
                    .ThenInclude(p => p.PostLocations)
                        .ThenInclude(l => l.Location)
                        .ThenInclude(l => l.Address)
                        .ThenInclude(l => l.City)

                .Include(c => c.CompanySkills)
                    .ThenInclude(ck => ck.Skill)
                .Include(c => c.CompanyIndustries)
                    .ThenInclude(ci => ci.Industry)
                .Include(c => c.Posts)
                    .ThenInclude(ck => ck.PostSkills)
                        .ThenInclude(ck => ck.Skill)
                .Include(c => c.Images)
                .Include(c => c.Followers)

                .SingleOrDefault(c => c.Slug == slug);

            if (company == null)
            {
                return NotFound();
            }

            // ✅ Đưa xuống đây, sau khi đã check null
            var locations = company.Locations
                .Where(l => l.Address != null && l.Address.City != null)
                .GroupBy(l => l.Address.City.Name)
                .ToDictionary(g => g.Key, g => g.Select(l => l.Address).ToList());

            var user = await _userManager.GetUserAsync(User);
            bool isFollowing = false;

            if (user != null)
            {
                isFollowing = await _dbContext.Followers
                    .AnyAsync(f => f.UserId == user.Id && f.CompanyId == company.CompanyId); // 🛠️ Sửa lại CountryId → CompanyId
            }
            ViewData["isFollowing"] = isFollowing;

            ViewData["company"] = company;
            ViewData["locations"] = locations;
            return View();
        }


        [HttpGet]
        [Route("{slug}/review")]
        public async Task<IActionResult> Review(string slug, int page = 1, int pageSize = 5)
        {
            var user = await _userManager.GetUserAsync(User);

            var company = _dbContext.Companies
                  .Include(c => c.Posts)
                .Include(c => c.Reviews)
                .Include(c => c.CompanyType)
                .Include(c => c.Locations)
                    .ThenInclude(cl => cl.Address)
                        .ThenInclude(a => a.City)

                .Include(c => c.Posts)
                    .ThenInclude(p => p.PostLevels)
                        .ThenInclude(l => l.Level)

                 .Include(c => c.Posts)
                    .ThenInclude(p => p.PostWorkTypes)
                        .ThenInclude(l => l.WorkType)
                .Include(c => c.Posts)
                    .ThenInclude(p => p.PostLocations)
                        .ThenInclude(l => l.Location)
                        .ThenInclude(l => l.Address)
                        .ThenInclude(l => l.City)

                .Include(c => c.CompanySkills)
                    .ThenInclude(ck => ck.Skill)
                .Include(c => c.CompanyIndustries)
                    .ThenInclude(ci => ci.Industry)
                .Include(c => c.Posts)
                    .ThenInclude(ck => ck.PostSkills)
                        .ThenInclude(ck => ck.Skill)
                .Include(c => c.Images)
                .Include(c => c.Followers)

                .SingleOrDefault(c => c.Slug == slug);




            var locations = company.Locations
                .Where(l => l.Address != null && l.Address.City != null) // Ensure Address and Province exist
                .GroupBy(l => l.Address.City.Name)
                .ToDictionary(g => g.Key, g => g.Select(l => l.Address).ToList());
            if (company == null)
            {
                return NotFound();
            }

            var totalReviews = company.Reviews.Count();
            var totalPages = (int)Math.Ceiling((double)totalReviews / pageSize);
            var pagedReviews = company.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


            bool isFollowing = false;
            if (user != null)
            {
                isFollowing = await _userService.IsFollowingAsync(user.Id, company.CompanyId);
            }


            var stats = _reviewStatsService.GetFullReviewStats(company.Reviews.ToList());
            ViewData["isFollowing"] = isFollowing;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["stats"] = stats;
            ViewData["company"] = company;
            ViewData["locations"] = locations;
            //ViewData["reviews"] = company.Reviews;
            return View(pagedReviews);
        }

        [HttpGet]
        [Route("{slug}/review/new")]
        public IActionResult New(string slug, int? star)
        {
            var company = _dbContext.Companies
        
                .FirstOrDefault(c => c.Slug == slug);

            if (company == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy công ty
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var review = new Review
            {
                UserId = userId,
                OverallRating = star ?? 0,
                CompanyId = company.CompanyId,
            };
            ViewData["company"] = company;
            return View(review);
        }

        [HttpPost]
        [Route("{slug}/review/new")]
        public async Task<IActionResult> New(string slug, Review newReview)
        {
            var company = await _dbContext.Companies
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (company == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            newReview.UserId = userId;
            newReview.CompanyId = company.CompanyId;

            if (ModelState.IsValid)
            {
                var (isRecommend, sentiment, modelName) = await GetSentimentFromPythonAsync(newReview.Summary);

                if (isRecommend != null)
                {
                    newReview.RecommendToFriends = isRecommend.Value;
                    newReview.Sentiment = sentiment;
                    newReview.SentimentModelName = modelName;
                }

                _dbContext.Reviews.Add(newReview);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "Company", new { area = "User/Companies", slug = slug });
            }

            ViewData["company"] = company;
            return View(newReview);
        }

        private async Task<(bool? isRecommend, string? sentiment, string? modelName)> GetSentimentFromPythonAsync(string summary)
        {
            using var client = new HttpClient();

            var payload = new { summary = summary };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:8000/api/predict/", content);
            if (!response.IsSuccessStatusCode) return (null, null, null);

            var responseContent = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(responseContent);

            bool isRecommend = doc.RootElement.GetProperty("is_recommend").GetBoolean();
            string sentiment = doc.RootElement.GetProperty("sentiment").GetString();
            string modelName = doc.RootElement.GetProperty("model_name").GetString();

            return (isRecommend, sentiment, modelName);
        }
 
        [HttpPost]
        [Route("{slug}/follow")]
        public IActionResult Follow(string slug)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var company = _dbContext.Companies.FirstOrDefault(e => e.Slug == slug);
            if (company == null)
            {
                return NotFound(); // Không tìm thấy công ty
            }

            var existingFollow = _dbContext.Followers.FirstOrDefault(f => f.CompanyId == company.CompanyId && f.UserId == userId);
            if (existingFollow == null)
            {
                _dbContext.Followers.Add(new Follower
                {
                    CompanyId = company.CompanyId,
                    UserId = userId
                });
                _dbContext.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Route("{slug}/unfollow")]
        public IActionResult UnFollow(string slug)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var company = _dbContext.Companies.FirstOrDefault(e => e.Slug == slug);
            if (company == null)
            {
                return NotFound(); // Không tìm thấy công ty
            }

            var follower = _dbContext.Followers.FirstOrDefault(f => f.CompanyId == company.CompanyId && f.UserId == userId);
            if (follower != null)
            {
                _dbContext.Followers.Remove(follower);
                _dbContext.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}