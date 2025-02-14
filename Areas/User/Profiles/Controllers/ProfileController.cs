using Microsoft.AspNetCore.Mvc;

namespace RecruitmentApp.Areas.User.Profiles.Controllers
{
    [Area("User/Profiles")]

    [Route("/profile")]
    [Route("/ho-so")]
    public class ProfileController : Controller
    {
        [HttpGet]
        [Route("")]
        [Route("dashboard")]
        [Route("thong-ke")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("manage-cv")]
        public IActionResult ManageCV()
        {
            return View();
        }

        [HttpGet]
        [Route("profile-cv")]
        public IActionResult ProfileCV()
        {
            return View();
        }

        [HttpGet]
        [Route("my-jobs")]
        public IActionResult MyJobs()
        {
            return View();
        }

        [HttpGet]
        [Route("my-jobs/applied")]
        public IActionResult MyJobsApplied()
        {
            return View();
        }

        [HttpGet]
        [Route("my-jobs/recent-viewed")]
        public IActionResult MyJobsRecentViewed()
        {
            return View();
        }


        [HttpGet]
        [Route("job-invitation")]
        public IActionResult JobInvitation()
        {
            return View();
        }


        [HttpGet]
        [Route("settings")]
        public IActionResult Settings()
        {
            return View();
        }
    }
}
