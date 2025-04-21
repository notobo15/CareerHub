using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.CompanyTypes.Controllers
{
    [Area("Admin/CompanyTypes")]
    [Route("/admin/title/[action]")]
    public class CompanyTypeController : Controller
    {
        private readonly AppDbContext _context;

        public CompanyTypeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Title
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản lý CompanyTypes";

            // Đọc TempData nếu có thông báo
            ViewBag.Message = TempData["SuccessMessage"];
            return View(await _context.CompanyTypes.ToListAsync());
        }

        // GET: Title/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi tiết CompanyTypes";

            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.CompanyTypes.FirstOrDefaultAsync(m => m.CompanyTypeId == id);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // GET: Title/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mới CompanyTypes";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyType companyType)
        {
            ViewData["Title"] = "Tạo mới Title";
            if (ModelState.IsValid)
            {
                _context.Add(companyType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo mới thành công (ID: {companyType.CompanyTypeId})!";
                return RedirectToAction(nameof(Index));
            }
            return View(companyType);
        }

        // GET: Title/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Chỉnh sửa CompanyTypes";
            if (id == null)
            {
                return NotFound();
            }

            var companyType = await _context.CompanyTypes.FindAsync(id);
            if (companyType == null)
            {
                return NotFound();
            }
            return View(companyType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyType companyType)
        {
            ViewData["Title"] = "Chỉnh sửa CompanyTypes";
            if (id != companyType.CompanyTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công (ID: {companyType.CompanyTypeId})!";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Exists(companyType.CompanyTypeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(companyType);
        }

        // GET: Title/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa CompanyType";

            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.CompanyTypes.FirstOrDefaultAsync(m => m.CompanyTypeId == id);
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
            var companyType = await _context.CompanyTypes.FindAsync(id);
            _context.CompanyTypes.Remove(companyType);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa thành công (ID: {id})!";

            return RedirectToAction(nameof(Index));
        }

        private bool Exists(int id)
        {
            return _context.CompanyTypes.Any(e => e.CompanyTypeId == id);
        }
    }
}
