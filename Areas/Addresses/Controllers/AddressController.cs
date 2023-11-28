using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Addresses.Controllers
{
    [Area("Addresses")]
    [Route("/admin/address/[action]/{id?}")]
    public class AddressController : Controller
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Skill
        public async Task<IActionResult> Index()
        {
            return View(await _context.Addresses
                .Include(a => a.Company)
                .Include(a => a.City)
                .Include(a => a.District)
                .Include(a => a.Ward)
                .ToListAsync());
        }

        // GET: Skill/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                .Include(a => a.Company)
                .Include(a => a.City)
                .Include(a => a.District)
                .Include(a => a.Ward)
                .FirstOrDefaultAsync(m => m.AddressId == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Skill/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Skill/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,ProvinceCode,DistrictCode,WardCode,DetailPosition,GgMapSrc")] Address address)
        {
          
            if (ModelState.IsValid)
            {

                address.GgMapSrc = GetSrcGgMap(address.GgMapSrc);

                var ward = _context.wards.FirstOrDefault(w => w.Code == address.WardCode);
                var district = _context.districts.FirstOrDefault(w => w.Code == address.DistrictCode);
                var province = _context.provinces.FirstOrDefault(w => w.Code == address.ProvinceCode);
                address.FullAddress = $"{address.DetailPosition}, {ward.FullName}, {district.FullName}, {province.FullName}";
                _context.Add(address);
                 await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(address);
        }

        // GET: Skill/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        // POST: Skill/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AddressId,CompanyId,ProvinceCode,DistrictCode,WardCode,DetailPosition,GgMapSrc")] Address address)
        {
            if (id != address.AddressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    address.GgMapSrc = GetSrcGgMap(address.GgMapSrc);

                    var ward = _context.wards.FirstOrDefault(w => w.Code == address.WardCode);
                    var district = _context.districts.FirstOrDefault(w => w.Code == address.DistrictCode);
                    var province = _context.provinces.FirstOrDefault(w => w.Code == address.ProvinceCode);
                    address.FullAddress = $"{address.DetailPosition}, {ward.FullName}, {district.FullName}, {province.FullName}";

                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(address.AddressId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(address);
        }

        // GET: Skill/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = await _context.Addresses
                .FirstOrDefaultAsync(m => m.AddressId == id);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // POST: Skill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skill = await _context.Addresses.FindAsync(id);
            _context.Addresses.Remove(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SkillExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressId == id);
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<District>> districts(string province)
        {
            if (!string.IsNullOrEmpty(province))
            {
                return Json(_context.districts.Where(d => d.ProvinceId == province));
            }
            return Json(_context.districts.ToList());
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<Ward>> wards(string district)
        {
            if (!string.IsNullOrEmpty(district))
            {
                return Json(_context.wards.Where(d => d.DistrictId == district));
            }
            return Json(_context.wards.ToList());
        }

        private  static string GetSrcGgMap(string str)
        {
            string srcAttributeValue = "";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(str);

            // Tìm thẻ iframe và lấy giá trị của thuộc tính src
            HtmlNode iframeNode = doc.DocumentNode.SelectSingleNode("//iframe");
            if (iframeNode != null)
            {
                srcAttributeValue = iframeNode.GetAttributeValue("src", "");
            }
            return srcAttributeValue;
        }
    }
}
