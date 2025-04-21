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
    [Area("Admin/Industries")]
    [Route("/admin/industries/[action]")]
    public class IndustryController : Controller
    {
        private readonly AppDbContext _context;

        public IndustryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Title
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản lý Industries";

            // Đọc TempData nếu có thông báo
            ViewBag.Message = TempData["SuccessMessage"];
            return View(await _context.Industries.ToListAsync());
        }

        // GET: Industries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi tiết Industries";

            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Industries.FirstOrDefaultAsync(m => m.IndustryId == id);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // GET: Title/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mới Industries";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Industry industry)
        {
            ViewData["Title"] = "Tạo mới Industries";
            if (ModelState.IsValid)
            {
                _context.Add(industry);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo mới thành công (ID: {industry.IndustryId})!";
                return RedirectToAction(nameof(Index));
            }
            return View(industry);
        }

        // GET: Title/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Chỉnh sửa Industries";
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Industries.FindAsync(id);
            if (title == null)
            {
                return NotFound();
            }
            return View(title);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Industry industry)
        {
            ViewData["Title"] = "Chỉnh sửa Industries";
            if (id != industry.IndustryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(industry);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công (ID: {industry.IndustryId})!";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitleExists(industry.IndustryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(industry);
        }

        // GET: Title/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Title";

            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Industries.FirstOrDefaultAsync(m => m.IndustryId == id);
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
            var title = await _context.Industries.FindAsync(id);
            _context.Industries.Remove(title);
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
