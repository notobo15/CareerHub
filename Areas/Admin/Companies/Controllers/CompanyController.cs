using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Areas.Admin.Companies.Models;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Admin.Companies.Controllers
{
    [Area("Admin/Companies")]
    [Route("/admin/[controller]/[action]/{id?}")]
    public class CompanyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CompanyController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public CompanyController(AppDbContext context, ILogger<CompanyController> logger, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }

        // GET: Company
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản Lý Công Ty";
            return View(await _context.Companies.OrderByDescending(c => c.UpdatedAt).ToListAsync());
        }

        // GET: Company/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Images)
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

         
            return View(company);
        }

        // GET: Company/Create
        public IActionResult Create()
        {
            var listSkill = _context.Skills.ToList();
            ViewData["list"] = new MultiSelectList(listSkill, "SkillId", "Name");
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,Slug,Name,Size,Description,Reason,Phone,Email,Type,Nation,OverTime,WorkingTime,LogoImage,CompanyUrl,CompanyFbUrl,SkillIds")] CreateCompany company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();

                if(company.SkillIds != null)
                {
                    foreach (var skillId in company.SkillIds)
                    {
                        _context.Add(new CompanySkills()
                        {
                            Company = company,
                            SkillID = skillId
                        });
                    }
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.CompanySkills)
                .Include(c => c.Images)
                .Include(c => c.Locations)
                .ThenInclude(c => c.Address)
                .ThenInclude(c => c.City)
                .ThenInclude(c => c.Districts)
                .ThenInclude(c => c.Wards)
                .FirstOrDefaultAsync(c => c.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            var recruiter = await _userManager.GetUsersInRoleAsync("Recruiter");

           

            var CreateCompany = new CreateCompany()
            {
                CompanyId = company.CompanyId,
                Name = company.Name,
                Addresses = company.Addresses,
                CompanyFbUrl = company.CompanyFbUrl,
                Email = company.Email,
                Nation = company.Nation,
                OverTime = company.OverTime,
                Phone = company.Phone,
                Size = company.Size,
                Slug = company.Slug,
                Reason = company.Reason,
                Type = company.Type,
                IsShowCompanyUrl = company.IsShowCompanyUrl,
                IsShowCompanyFbUrl = company.IsShowCompanyFbUrl,
                WorkingTime = company.WorkingTime,
                CompanyUrl = company.CompanyUrl,
                Description = company.Description,
                LogoImage = company.LogoImage,
                SkillIds = company.CompanySkills.Select(e => e.SkillID).ToArray(),
                Countries = _context.Countries
                    .Select(c => new SelectListItem {
                        Value = c.CountryId.ToString(),
                        Text = c.Name
                    }).ToList(),
                RecruiterIds = recruiter.Select(e => new SelectListItem
                {
                    Value = e.Id,
                    Text = e.FullName
                }).ToList(),
                Skills = new MultiSelectList(_context.Skills.ToList(), "SkillId", "Name"),

                Images = company.Images,
                CreateLocation = new CreateLocationViewModel
                {
                    CompanyId = company.CompanyId,
                    Provinces = _context.Provinces
                        .Select(p => new SelectListItem
                        {
                            Value = p.Code,
                            Text = p.FullName
                        }).ToList(),
                    Districts = new List<SelectListItem>(),
                    Wards = new List<SelectListItem>()
                },
                Locations = company.Locations.ToList(),
            };

            return View(CreateCompany);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Slug,Name,Size,IsShow,IsClose,Description,Reason,Phone,Email,Type,Nation,OverTime,WorkingTime,File,CompanyUrl,CompanyFbUrl, SkillIds")] CreateCompany NewCompany)
        {
            if (id != NewCompany.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var company = await _context.Companies
                        .Include(c => c.CompanySkills)
                        .FirstOrDefaultAsync(c => c.CompanyId == id);

                    if (company == null)
                    {
                        return NotFound();
                    }
                    if (NewCompany.File != null && NewCompany.File.Length > 0)
                    {
                        var fileNameLogo = NewCompany.Slug + "-" + NewCompany.CompanyId+ Path.GetExtension(NewCompany.File.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/company-logo", fileNameLogo);

                        company.LogoImage = fileNameLogo;

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            NewCompany.File.CopyTo(stream);
                        }
                    }

                    company.CompanyId = NewCompany.CompanyId;
                    company.Name = NewCompany.Name;
                    company.Addresses = NewCompany.Addresses;
                    company.CompanyFbUrl = NewCompany.CompanyFbUrl;
                    company.Email = NewCompany.Email;
                    company.Nation = NewCompany.Nation;
                    company.OverTime = NewCompany.OverTime;
                    company.Phone = NewCompany.Phone;
                    company.Size = NewCompany.Size;
                    company.Slug = NewCompany.Slug;
                    company.Reason = NewCompany.Reason;
                    company.Type = NewCompany.Type;
                    company.WorkingTime = NewCompany.WorkingTime;
                    company.CompanyUrl = NewCompany.CompanyUrl;
                    company.Description = NewCompany.Description;
                    

                    // if skill== null
                    if (NewCompany.SkillIds == null) NewCompany.SkillIds = new int[] { };

                    var oldSkills = company.CompanySkills.Select( e=> e.SkillID ).ToArray();

                    var newSkills = NewCompany.SkillIds;

                    var removeSkills = from c in company.CompanySkills
                                   where (!newSkills.Contains(c.SkillID))
                                   select c;
                    _context.RemoveRange(removeSkills);
                    var addSkills = from c in  newSkills
                                    where(!oldSkills.Contains(c))
                                    select c;


                    foreach (var skillId in addSkills)
                    {
                        _context.CompanySkills.Add(new CompanySkills()
                        {
                            CompanyID = id,
                            SkillID = skillId,
                        });
                    }
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(NewCompany.CompanyId))
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
            return View(NewCompany);
        }

        // GET: Company/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
       


        [HttpPost]
        public async Task<IActionResult> SaveCompany(Company company, string SavedImageFileNames, List<IFormFile> newImages)
        {
            if (!ModelState.IsValid) return View(company);

            // Tạo hoặc cập nhật company
            if (company.CompanyId == 0)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Update(company);
                await _context.SaveChangesAsync();
            }

            // Đường dẫn lưu ảnh
            var uploadPath = Path.Combine(_env.WebRootPath, "images", "sliders");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            // Xử lý ảnh mới
            if (newImages != null && newImages.Any())
            {
                foreach (var file in newImages)
                {
                    if (file.Length > 0)
                    {
                        var fileName = $"{company.Slug}-{company.CompanyId}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Thêm ảnh mới vào cơ sở dữ liệu
                        _context.Images.Add(new Image
                        {
                            FileName = fileName,
                            FullPath = $"/company-images/{fileName}",
                            CompanyId = company.CompanyId,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        });
                    }
                }
            }

            // Xử lý ảnh hiện có
            var allFileNames = (SavedImageFileNames ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries);
            var existingImages = await _context.Images.Where(i => i.CompanyId == company.CompanyId).ToListAsync();

            // Đánh dấu xóa ảnh không còn trong danh sách
            foreach (var img in existingImages)
            {
                if (!allFileNames.Contains(img.FileName))
                {
                    img.IsDeleted = true;
                    img.UpdatedAt = DateTime.Now;
                }
            }

            // Khôi phục ảnh nếu được thêm lại (trường hợp hiếm)
            foreach (var name in allFileNames)
            {
                var image = existingImages.FirstOrDefault(x => x.FileName == name);
                if (image != null && image.IsDeleted)
                {
                    image.IsDeleted = false;
                    image.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Lưu công ty và ảnh thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCompanyImages([FromForm] CompanyImagesViewModel model)
        {
            var company = await _context.Companies
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CompanyId == model.CompanyId);

            if (company == null)
                return NotFound();

            // Xóa ảnh
            if (!string.IsNullOrEmpty(model.DeletedImageIds))
            {
                var deleteIds = model.DeletedImageIds.Split(',').Select(int.Parse).ToList();
                var imagesToDelete = _context.Images.Where(img => deleteIds.Contains(img.ImageId));
                _context.Images.RemoveRange(imagesToDelete);
            }

            // Thêm ảnh mới
            if (model.NewImages != null && model.NewImages.Count > 0)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "images/sliders");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                foreach (var file in model.NewImages)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _context.Images.Add(new Image
                    {
                        FileName = fileName,
                        FullPath = "/images/sliders/" + fileName,
                        CompanyId = company.CompanyId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Order = 9999
                    });
                }
            }

            await _context.SaveChangesAsync();

            // Cập nhật thứ tự
            if (!string.IsNullOrEmpty(model.ImageOrder))
            {
                var idList = model.ImageOrder.Split(',').Select(int.Parse).ToList();

                for (int i = 0; i < idList.Count; i++)
                {
                    var img = await _context.Images.FindAsync(idList[i]);
                    if (img != null && img.CompanyId == company.CompanyId)
                    {
                        img.Order = i;
                        img.UpdatedAt = DateTime.Now;
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { id = model.CompanyId, tab = "images" });
        }

        public IActionResult CreateLocation(int companyId)
        {
            var model = new CreateLocationViewModel
            {
                CompanyId = companyId,
                Provinces = _context.Provinces
                    .Select(p => new SelectListItem
                    {
                        Value = p.Code,
                        Text = p.Name
                    }).ToList(),
                Districts = new List<SelectListItem>(),
                Wards = new List<SelectListItem>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(CreateLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Provinces = _context.Provinces
                    .Select(p => new SelectListItem { Value = p.Code, Text = p.Name }).ToList();

                if (!string.IsNullOrEmpty(model.ProvinceCode))
                {
                    model.Districts = _context.Districts
                        .Where(d => d.ProvinceId == model.ProvinceCode)
                        .Select(d => new SelectListItem { Value = d.Code, Text = d.Name }).ToList();
                }

                if (!string.IsNullOrEmpty(model.DistrictCode))
                {
                    model.Wards = _context.Wards
                        .Where(w => w.DistrictId == model.DistrictCode)
                        .Select(w => new SelectListItem { Value = w.Code, Text = w.Name }).ToList();
                }

                return View(model);
            }
            var province = await _context.Provinces.FindAsync(model.ProvinceCode);
            var district = await _context.Districts.FindAsync(model.DistrictCode);
            var ward = await _context.Wards.FindAsync(model.WardCode);

            // Ghép địa chỉ đầy đủ
            var fullAddress = $"{model.DetailPosition}, {ward?.Name}, {district?.Name}, {province?.Name}";
            var encodedAddress = Uri.EscapeDataString(fullAddress);
            var address = new Address
            {
                ProvinceCode = model.ProvinceCode,
                DistrictCode = model.DistrictCode,
                WardCode = model.WardCode,
                DetailPosition = model.DetailPosition,
                FullAddress = fullAddress,
                GgMapSrc = $"https://maps.google.com/?q={encodedAddress}"
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            _context.Locations.Add(new Location
            {
                CompanyId = model.CompanyId,
                AddressId = address.AddressId
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Company", new { id = model.CompanyId, tab = "locations" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocation(LocationEditViewModel model)
        {
            var location = await _context.Locations
                .Include(l => l.Address)
                .FirstOrDefaultAsync(l => l.LocationId == model.LocationId);

            if (location == null)
                return NotFound();

            var province = await _context.Provinces.FindAsync(model.ProvinceCode);
            var district = await _context.Districts.FindAsync(model.DistrictCode);
            var ward = await _context.Wards.FindAsync(model.WardCode);

            // Ghép địa chỉ đầy đủ
            var fullAddress = $"{model.DetailPosition}, {ward?.Name}, {district?.Name}, {province?.Name}";
            var encodedAddress = Uri.EscapeDataString(fullAddress);

            var address = location.Address;

            address.ProvinceCode = model.ProvinceCode;
            address.DistrictCode = model.DistrictCode;
            address.WardCode = model.WardCode;
            address.DetailPosition = model.DetailPosition;
            address.FullAddress = fullAddress;
            address.GgMapSrc = $"https://maps.google.com/?q={encodedAddress}";

            _context.Update(address);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = location.CompanyId, tab = "locations" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLocation(int locationId, int companyId)
        {
            var location = await _context.Locations
                .Include(l => l.Address)
                .FirstOrDefaultAsync(l => l.LocationId == locationId && l.CompanyId == companyId);

            if (location == null)
            {
                return NotFound();
            }

            // Xoá location
            _context.Locations.Remove(location);

            // (Tuỳ chọn) Xoá cả địa chỉ nếu không dùng nơi khác
            if (location.Address != null)
            {
                _context.Addresses.Remove(location.Address);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = companyId, tab = "locations" });
        }
    }
}

