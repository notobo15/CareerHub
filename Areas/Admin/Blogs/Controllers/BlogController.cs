using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Areas.Admin.Blogs.ViewModels;
using RecruitmentApp.Models;
using RecruitmentApp.Services;
using RecruitmentApp.Utilities;

namespace RecruitmentApp.Areas.Admin.Blogs.Controllers
{
    [Area("Admin/Blogs")]
    [Route("/admin/blog/[action]/{id?}")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UploadFileService _uploadService;
        public BlogController(AppDbContext context, UploadFileService uploadService)
        {
            _context = context;
            _uploadService = uploadService;
        }

        // GET: Blog
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string search = null)
        {
            ViewData["Title"] = "Quản lý Blog";
            ViewBag.Message = TempData["SuccessMessage"];

            var query = _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsQueryable();

            // Tìm kiếm theo Title hoặc Author.Name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    (b.Author != null && b.Author.FullName.Contains(search))
                );
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages > 0 ? totalPages : 1;

            var blogs = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.TotalItems = totalItems;

            return View(blogs);
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi tiết Blog";

            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BlogId == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blog/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mới Blog";

            var viewModel = new BlogViewModel
            {
                Categories = _context.BlogCategories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogViewModel model)
        {
            ViewData["Title"] = "Tạo mới Blog";

            if (!ModelState.IsValid)
            {
                // Gán lại danh sách danh mục nếu có lỗi form
                model.Categories = _context.BlogCategories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name,
                });
                return View(model);
            }
            var imageUrl = await _uploadService.UploadImageAsync(model.ThumbnailImage, "uploads/blogs");

            // Chuyển từ ViewModel sang Entity
            var blog = new Blog
            {
                Title = model.Title,
                Content = model.Content,
                Description = model.Description,
                Slug = AppUtilities.GenerateSlug(model.Title, id: model.BlogId),
                ReadTimeMinutes = CalculateReadTime(model.Content),
                ThumbnailUrl = imageUrl,
                CategoryId = model.CategoryId,
                AuthorId = model.AuthorId,
                IsPublished = model.IsPublished,
                CreatedAt = DateTime.Now
            };

            _context.Add(blog);
            await _context.SaveChangesAsync();

            blog.Slug = AppUtilities.GenerateSlug(blog.Title, id: blog.BlogId);

            _context.Update(blog);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Tạo mới thành công (ID: {blog.BlogId})!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Blog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Chỉnh sửa Blog";

            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var viewModel = new BlogViewModel
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Content = blog.Content,
                Description = blog.Description,
                Slug = AppUtilities.GenerateSlug(blog.Title, id: blog.BlogId),
                ThumbnailUrl = blog.ThumbnailUrl,
                IsPublished = blog.IsPublished,
                CategoryId = blog.CategoryId,
                AuthorId = blog.AuthorId,
                Categories = _context.BlogCategories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogViewModel model)
        {
            ViewData["Title"] = "Chỉnh sửa Blog";
            if (id != model.BlogId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = _context.BlogCategories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    });
                return View(model);
            }

           

            var blog = await _context.Blogs.FindAsync(model.BlogId);
            if (blog == null)
            {
                return NotFound();
            }
         
            if (model.ThumbnailImage != null && model.ThumbnailImage.Length > 0)
            {
                _uploadService.DeleteImage(blog.ThumbnailUrl); // 👈 gọi service

                blog.ThumbnailUrl = await _uploadService.UploadImageAsync(model.ThumbnailImage, "uploads/blogs");
            }


            blog.Title = model.Title;
            blog.Content = model.Content;
            blog.Description = model.Description;
            blog.Slug = AppUtilities.GenerateSlug(blog.Title, id: blog.BlogId);
            blog.ReadTimeMinutes = CalculateReadTime(blog.Content);
            blog.CategoryId = model.CategoryId;
            blog.AuthorId = model.AuthorId;
            blog.IsPublished = model.IsPublished;
            blog.UpdatedAt = DateTime.Now;

            _context.Update(blog);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cập nhật thành công (ID: {blog.BlogId})!";
            return RedirectToAction(nameof(Index));

        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Blog";

            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BlogId == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa thành công (ID: {id})!";
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }

        private int CalculateReadTime(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return 1;

            // Tách từ theo khoảng trắng
            var wordCount = content.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;

            // Trung bình 200 từ/phút
            double minutes = wordCount / 200.0;

            return Math.Max(1, (int)Math.Ceiling(minutes)); // Tối thiểu 1 phút
        }
    }
}
