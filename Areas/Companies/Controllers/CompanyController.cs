using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Areas.Companies.Models;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.Companies.Controllers
{
    [Area("Companies")]
    [Route("/Admin/[controller]/[action]/{id?}")]
    public class CompanyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(AppDbContext context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Company
        public async Task<IActionResult> Index()
        {
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
                .FirstOrDefaultAsync(c => c.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            var CreateCompany = new CreateCompany ()
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
                WorkingTime = company.WorkingTime,
                CompanyUrl = company.CompanyUrl,
                Description = company.Description,
                LogoImage = company.LogoImage,
                SkillIds = company.CompanySkills.Select(e => e.SkillID).ToArray(),

            };


            var listSkill = _context.Skills.ToList();
            ViewData["list"] = new MultiSelectList(listSkill, "SkillId", "Name");

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
        public IActionResult Follow(string userId,int? companyId)
        {
           
            _logger.LogInformation(""+userId);
            _logger.LogInformation(""+ companyId);
           if(string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", controllerName: "Account", new { area = "Identity"});
            }
           if(companyId != null)
            {
                _context.Followers.Add(new Follower
                { 
                    
                    Company = _context.Companies.FirstOrDefault(e => e.CompanyId ==companyId),
                   
                    UserID = userId
                });
            }
            _context.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public IActionResult UnFollow(string userId, int? companyId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", controllerName: "Account", new { area = "Identity" });
            }
            if (companyId != null)
            {
                var follower = _context.Followers.FirstOrDefault(e => e.CompanyID ==companyId && e.UserID == userId);
                _context.Followers.Remove(follower);
            }
            _context.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }

}
