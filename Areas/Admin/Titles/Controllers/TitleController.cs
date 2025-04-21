using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.Titles.Controllers
{
    [Area("Admin/Titles")]
    [Route("/admin/title/[action]")]
    public class TitleController : Controller
    {
        private readonly AppDbContext _context;

        public TitleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Title
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản lý Title";

            // Đọc TempData nếu có thông báo
            ViewBag.Message = TempData["SuccessMessage"];
            return View(await _context.Titles.ToListAsync());
        }

        // GET: Title/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi tiết Title";

            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles.FirstOrDefaultAsync(m => m.Id == id);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // GET: Title/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mới Title";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Title title)
        {
            ViewData["Title"] = "Tạo mới Title";
            if (ModelState.IsValid)
            {
                _context.Add(title);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo mới thành công (ID: {title.Id})!";
                return RedirectToAction(nameof(Index));
            }
            return View(title);
        }

        // GET: Title/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Chỉnh sửa Title";
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles.FindAsync(id);
            if (title == null)
            {
                return NotFound();
            }
            return View(title);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Title title)
        {
            ViewData["Title"] = "Chỉnh sửa Title";
            if (id != title.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(title);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công (ID: {title.Id})!";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitleExists(title.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(title);
        }

        // GET: Title/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Title";

            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles.FirstOrDefaultAsync(m => m.Id == id);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // POST: Title/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var title = await _context.Titles.FindAsync(id);
            _context.Titles.Remove(title);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa thành công (ID: {id})!";

            return RedirectToAction(nameof(Index));
        }

        private bool TitleExists(int id)
        {
            return _context.Titles.Any(e => e.Id == id);
        }
    }
}
