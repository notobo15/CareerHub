using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;
using RecruitmentApp.Services;

namespace RecruitmentApp.Areas.User.Blogs.Controllers
{
    [Area("User/Blogs")]
    [Route("blogs")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // GET: /blogs
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var blogs = await _dbContext.Blogs
                .Include(b => b.Author)
                .Where(b => b.IsPublished)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var categories = await _dbContext.BlogCategories.ToListAsync();

            if (categories == null)
            {
                return NotFound();
            }
            ViewBag.Categories = categories;
            return View(blogs);
        }
        [HttpGet("{slug}")]
        public async Task<IActionResult> Category(string slug, int page = 1)
        {
            const int pageSize = 6;

            var category = await _dbContext.BlogCategories
                .Include(c => c.Blogs.Where(b => b.IsPublished))
                    .ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category == null)
                return NotFound();

            var totalPosts = category.Blogs.Count;
            var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

            var blogs = category.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CategorySlug = slug;

            category.Blogs = blogs;

            return View(category);
        }
        // GET: /blogs/{slug}
        [HttpGet("{category}/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();

            var blog = await _dbContext.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.BlogViews)
                .FirstOrDefaultAsync(b => b.Slug == slug && b.IsPublished);

            if (blog == null)
            {
                return NotFound();
            }
            var categories = await _dbContext.BlogCategories
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(blog);
        }


        [HttpPost]
        [Route("/posts/track-view/{postId}")]
        public async Task<IActionResult> TrackView(int postId)
        {
            string? userId = User.Identity.IsAuthenticated
                ? _userManager.GetUserId(User)
                : null;

            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Tính từ 24h trước
            var timeLimit = DateTime.UtcNow.AddHours(-24);

            var recentViews = await _dbContext.PostViews
                .Where(v => v.PostId == postId && v.ViewedAt >= timeLimit &&
                       (v.UserId == userId || (userId == null && v.IpAddress == ip)))
                .ToListAsync();

            if (recentViews.Count >= 5)
            {
                return Ok(new { success = false, reason = "limit_exceeded" });
            }

            // Lưu view
            var view = new PostView
            {
                PostId = postId,
                UserId = userId,
                IpAddress = ip,
                ViewedAt = DateTime.UtcNow
            };

            _dbContext.PostViews.Add(view);

            // Tăng lượt xem nếu hợp lệ
            var post = await _dbContext.Posts.FindAsync(postId);
            if (post != null)
            {
                post.ViewTotal++;
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
