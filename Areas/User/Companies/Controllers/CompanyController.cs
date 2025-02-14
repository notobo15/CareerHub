using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Models;

namespace RecruitmentApp.Areas.User.Companies.Controllers
{
  [Area("User/Companies")]

  [Route("/companies")]
  [Route("/nha-tuyen-dung")]
  public class CompanyController : Controller
  {
    private readonly AppDbContext _context;

    public CompanyController(AppDbContext context)
    {
      _context = context;
    }

    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {
        return View();

    }


        [HttpGet]
        [Route("{name}")]
        public IActionResult Detail(string name)
        {
            return View();
        }


        [HttpGet]
        [Route("{slug}/review")]
        public IActionResult Review(string slug)
        {
            return View();
        }

        [HttpGet]
        [Route("{slug}/review/new")]
        public IActionResult New(string slug)
        {
            return View();
        }

        [HttpPost]
        [Route("{slug}/review/new")]
        public IActionResult New(string slug, string model)
        {
            return View();
        }
    }
}