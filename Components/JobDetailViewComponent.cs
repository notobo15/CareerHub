using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.DTOs;

namespace RecruitmentApp.Components
{
    public class JobDetailViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PostDTO post)
        {
            return View(post);
        }
    }
}
