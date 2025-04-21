using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Data;
using RecruitmentApp.Models;
using System.Linq;

namespace RecruitmentApp.Areas.Admin.Recruiters.Controllers
{
    [Authorize(Roles = RoleName.Recuiter)]
    [Area("Admin/Recruiters")]
    [Route("/nha-tuyen-dung/apply/[action]/{id?}")]
    public class ManageApplyController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public ManageApplyController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            string id = _userManager.GetUserId(User);
            var result = _appDbContext.ApplyPosts.Where(a => a.UserID == id).ToList();
            return View(result);
        }


    }
}
