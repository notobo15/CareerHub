using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Areas.Companies.Models;
using RecruitmentApp.Data;
using RecruitmentApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Areas.Recruiters.Controllers
{
    [Area("Recruiters")]

    [Authorize(Roles = RoleName.Recuiter)]
    [Route("/nha-tuyen-dung/[action]/{id?}")]

    public class ManageInfoController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public ManageInfoController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        [Route("/nha-tuyen-dung")]
        public IActionResult Index()
        {

            var company = _appDbContext.Companies
                .FirstOrDefault();
            
            return View(company);
        }
        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _appDbContext.Companies
                .Include(c => c.CompanySkills)
                .FirstOrDefaultAsync(c => c.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

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
                WorkingTime = company.WorkingTime,
                CompanyUrl = company.CompanyUrl,
                Description = company.Description,
                LogoImage = company.LogoImage,
                SkillIds = company.CompanySkills.Select(e => e.SkillID).ToArray(),

            };


            var listSkill = _appDbContext.Skills.ToList();
            ViewData["list"] = new MultiSelectList(listSkill, "SkillId", "Name");

            return View(CreateCompany);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Slug,Name,Size,Description,Reason,Phone,Email,Type,Nation,OverTime,WorkingTime,LogoImage,CompanyUrl,CompanyFbUrl, SkillIds")] CreateCompany NewCompany)
        {
            if (id != NewCompany.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var company = await _appDbContext.Companies
                        .Include(c => c.CompanySkills)
                        .FirstOrDefaultAsync(c => c.CompanyId == id);

                    if (company == null)
                    {
                        return NotFound();
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
                    company.LogoImage = NewCompany.LogoImage;

                    // if skill== null
                    if (NewCompany.SkillIds == null) NewCompany.SkillIds = new int[] { };

                    var oldSkills = company.CompanySkills.Select(e => e.SkillID).ToArray();

                    var newSkills = NewCompany.SkillIds;

                    var removeSkills = from c in company.CompanySkills
                                       where (!newSkills.Contains(c.SkillID))
                                       select c;
                    _appDbContext.RemoveRange(removeSkills);
                    var addSkills = from c in newSkills
                                    where (!oldSkills.Contains(c))
                                    select c;


                    foreach (var skillId in addSkills)
                    {
                        _appDbContext.CompanySkills.Add(new CompanySkills()
                        {
                            CompanyID = id,
                            SkillID = skillId,
                        });
                    }
                    _appDbContext.Update(company);
                    await _appDbContext.SaveChangesAsync();
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
        private bool CompanyExists(int id)
        {
            return _appDbContext.Companies.Any(e => e.CompanyId == id);
        }
    }
}
