﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.Levels.Controllers
{
    [Area("Admin/Levels")]
    [Route("/admin/level/[action]/{id?}")]
    public class LevelController : Controller
    {
        private readonly AppDbContext _context;

        public LevelController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Level
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string search = null)
        {
            ViewData["Title"] = "Quản lý Level";
            ViewBag.Message = TempData["SuccessMessage"];

            var query = _context.Levels.AsQueryable();

            // Tìm kiếm theo Name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l => l.Name.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages > 0 ? totalPages : 1;

            var levels = await query
                .OrderByDescending(l => l.LevelId) // Hoặc OrderBy(l => l.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Truyền dữ liệu cho View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.TotalItems = totalItems;

            return View(levels);
        }


        // GET: Level/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi tiết Level";
            if (id == null)
            {
                return NotFound();
            }

            var level = await _context.Levels
                .FirstOrDefaultAsync(m => m.LevelId == id);
            if (level == null)
            {
                return NotFound();
            }

            return View(level);
        }

        // GET: Level/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mới Level";
            return View();
        }

        // POST: Level/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelId,Name")] Level level)
        {
            ViewData["Title"] = "Tạo mới Level";
            if (ModelState.IsValid)
            {
                _context.Add(level);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo mới thành công (ID: {level.LevelId})!";
                return RedirectToAction(nameof(Index));
            }
            return View(level);
        }

        // GET: Level/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Chỉnh sửa Level";

            if (id == null)
            {
                return NotFound();
            }

            var level = await _context.Levels.FindAsync(id);
            if (level == null)
            {
                return NotFound();
            }
            return View(level);
        }

        // POST: Level/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevelId,Name")] Level level)
        {
            ViewData["Title"] = "Chỉnh sửa Level";

            if (id != level.LevelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(level);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công (ID: {level.LevelId})!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LevelExists(level.LevelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(level);
        }

        // GET: Level/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xóa Level";

            if (id == null)
            {
                return NotFound();
            }

            var level = await _context.Levels
                .FirstOrDefaultAsync(m => m.LevelId == id);
            if (level == null)
            {
                return NotFound();
            }

            return View(level);
        }

        // POST: Level/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var level = await _context.Levels.FindAsync(id);
            _context.Levels.Remove(level);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa thành công (ID: {id})!";
            return RedirectToAction(nameof(Index));
        }

        private bool LevelExists(int id)
        {
            return _context.Levels.Any(e => e.LevelId == id);
        }
    }
}
