using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.WorkTypes.Controllers
{
    [Area("Admin/WorkTypes")]
    [Route("/admin/work-types/[action]")]
    public class WorkTypeController : Controller
    {
        private readonly AppDbContext _context;

        public WorkTypeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: WorkType
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản lý hình thức làm việc";
            ViewBag.Message = TempData["SuccessMessage"];
            return View(await _context.WorkTypes.ToListAsync());
        }

        // GET: WorkType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Title"] = "Chi tiết hình thức làm việc";

            if (id == null)
                return NotFound();

            var workType = await _context.WorkTypes.FirstOrDefaultAsync(w => w.WorkTypeId == id);
            if (workType == null)
                return NotFound();

            return View(workType);
        }

        // GET: WorkType/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo mới hình thức làm việc";
            return View();
        }

        // POST: WorkType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkTypeId,Name,Slug")] WorkType workType)
        {
            ViewData["Title"] = "Tạo mới hình thức làm việc";

            if (ModelState.IsValid)
            {
                _context.Add(workType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo thành công (ID: {workType.WorkTypeId})!";
                return RedirectToAction(nameof(Index));
            }

            return View(workType);
        }

        // GET: WorkType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Title"] = "Chỉnh sửa hình thức làm việc";

            if (id == null)
                return NotFound();

            var workType = await _context.WorkTypes.FindAsync(id);
            if (workType == null)
                return NotFound();

            return View(workType);
        }

        // POST: WorkType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkTypeId,Name,Slug")] WorkType workType)
        {
            ViewData["Title"] = "Chỉnh sửa hình thức làm việc";

            if (id != workType.WorkTypeId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công (ID: {workType.WorkTypeId})!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkTypeExists(workType.WorkTypeId))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(workType);
        }

        // GET: WorkType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Title"] = "Xoá hình thức làm việc";

            if (id == null)
                return NotFound();

            var workType = await _context.WorkTypes.FirstOrDefaultAsync(w => w.WorkTypeId == id);
            if (workType == null)
                return NotFound();

            return View(workType);
        }

        // POST: WorkType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workType = await _context.WorkTypes.FindAsync(id);
            _context.WorkTypes.Remove(workType);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xoá thành công (ID: {id})!";
            return RedirectToAction(nameof(Index));
        }

        private bool WorkTypeExists(int id)
        {
            return _context.WorkTypes.Any(e => e.WorkTypeId == id);
        }
    }
}
