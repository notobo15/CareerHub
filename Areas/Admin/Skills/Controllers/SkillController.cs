﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.Skills.Controllers
{
    [Area("Admin/Skills")]
    [Route("/admin/skill/[action]/{id?}")]
    public class SkillController : Controller
    {
        private readonly AppDbContext _context;

        public SkillController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Skill
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string search = null)
        {
            ViewData["Title"] = "Quản lý Skill";
            ViewBag.Message = TempData["SuccessMessage"];

            var query = _context.Skills.AsQueryable();

            // Nếu có tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Name.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages > 0 ? totalPages : 1;

            var skills = await query
                .OrderByDescending(s => s.SkillId) // Hoặc OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.TotalItems = totalItems;

            return View(skills);
        }


        // GET: Skill/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi tiết Skill";

            if (id == null) return NotFound();

            var skill = await _context.Skills.FirstOrDefaultAsync(m => m.SkillId == id);
            if (skill == null) return NotFound();

            return View(skill);
        }

        // GET: Skill/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mới Skill";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SkillId,Name")] Skill skill)
        {
            ViewData["Title"] = "Tạo mới Skill";
            if (ModelState.IsValid)
            {
                _context.Add(skill);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo mới thành công (ID: {skill.SkillId})!";
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Skill/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Chỉnh sửa Skill";

            if (id == null) return NotFound();

            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();

            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SkillId,Name")] Skill skill)
        {
            ViewData["Title"] = "Chỉnh sửa Skill";

            if (id != skill.SkillId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skill);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công (ID: {skill.SkillId})!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(skill.SkillId)) return NotFound();
                    else throw;
                }
            }
            return View(skill);
        }

        // GET: Skill/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Skill";

            if (id == null) return NotFound();

            var skill = await _context.Skills.FirstOrDefaultAsync(m => m.SkillId == id);
            if (skill == null) return NotFound();

            return View(skill);
        }

        // POST: Skill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa thành công (ID: {id})!";
            return RedirectToAction(nameof(Index));
        }

        private bool SkillExists(int id)
        {
            return _context.Skills.Any(e => e.SkillId == id);
        }
    }
}
