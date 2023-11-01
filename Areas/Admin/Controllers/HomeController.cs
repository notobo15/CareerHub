using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Data;

namespace RecruitmentApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin")]
    [Authorize(Roles = RoleName.Administrator)]
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}