using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.Countries.Controllers
{
    [Area("Admin/Countries")]
    [Route("/admin/country/[action]")]
    public class CountryController : Controller
    {
        private readonly AppDbContext _context;

        public CountryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Country
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản lý Quốc gia";
            ViewBag.Message = TempData["SuccessMessage"];
            return View(await _context.Countries.ToListAsync());
        }

        // GET: Country/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi tiết Quốc gia";

            if (id == null) return NotFound();

            var country = await _context.Countries
                .Include(c => c.Companies) // nếu cần load liên quan
                .FirstOrDefaultAsync(m => m.CountryId == id);

            if (country == null) return NotFound();

            return View(country);
        }

        // GET: Country/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mới Quốc gia";
            return View();
        }

        // POST: Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Slug,ISOCode")] Country country)
        {
            ViewData["Title"] = "Tạo mới Quốc gia";
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo mới thành công (ID: {country.CountryId})!";
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Country/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Chỉnh sửa Quốc gia";
            if (id == null) return NotFound();

            var country = await _context.Countries.FindAsync(id);
            if (country == null) return NotFound();

            return View(country);
        }

        // POST: Country/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CountryId,Name,Slug,ISOCode")] Country country)
        {
            ViewData["Title"] = "Chỉnh sửa Quốc gia";
            if (id != country.CountryId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công (ID: {country.CountryId})!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryId)) return NotFound();
                    else throw;
                }
            }
            return View(country);
        }

        // GET: Country/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xoá Quốc gia";
            if (id == null) return NotFound();

            var country = await _context.Countries.FirstOrDefaultAsync(m => m.CountryId == id);
            if (country == null) return NotFound();

            return View(country);
        }

        // POST: Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xoá thành công (ID: {id})!";
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.CountryId == id);
        }
    }
}
