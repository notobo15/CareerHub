using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Areas.Admin.Titles.Controllers
{
    [Area("Admin/Provinces")]
    [Route("/admin/province/[action]")]
    public class ProvinceController : Controller
    {
        private readonly AppDbContext _context;

        public ProvinceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /admin/province/index
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Danh sách tỉnh/thành";
            ViewBag.Message = TempData["SuccessMessage"];
            var provinces = await _context.Provinces.ToListAsync();
            return View(provinces);
        }

        // GET: /admin/province/details?code=01
        public async Task<IActionResult> Details(string code)
        {
            ViewData["Title"] = "Chi tiết tỉnh/thành";

            if (string.IsNullOrEmpty(code))
                return NotFound();

            var province = await _context.Provinces
                .Include(p => p.Districts)
                    .ThenInclude(d => d.Wards)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (province == null)
                return NotFound();

            return View(province);
        }

        // GET: /admin/province/edit?code=01
        public async Task<IActionResult> Edit(string code)
        {
            ViewData["Title"] = "Chỉnh sửa tỉnh/thành";

            if (string.IsNullOrEmpty(code))
                return NotFound();

            var province = await _context.Provinces.FindAsync(code);
            if (province == null)
                return NotFound();

            return View(province);
        }

        // POST: /admin/province/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string code, [Bind("Code,Name,FullName,NameEn,FullNameEn,CodeName,administrative_unit_id,administrative_region_id,Slug")] Province model)
        {
            ViewData["Title"] = "Chỉnh sửa tỉnh/thành";

            if (code != model.Code)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công ({model.Name})!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinceExists(model.Code))
                        return NotFound();
                    throw;
                }
            }

            return View(model);
        }

        private bool ProvinceExists(string code)
        {
            return _context.Provinces.Any(p => p.Code == code);
        }
    }
}
