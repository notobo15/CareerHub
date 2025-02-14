using Microsoft.AspNetCore.Mvc;

namespace RecruitmentApp.Areas.User.Jobs.Controllers
{
    [Area("User/Jobs")]

    [Route("/jobs")]
    [Route("/viec-lam")]
    public class ProfileController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

    }
}
