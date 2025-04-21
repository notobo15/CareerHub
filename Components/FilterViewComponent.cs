using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.DTOs;
using RecruitmentApp.ModelViews;

namespace RecruitmentApp.Components
{
    public class FilterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(FilterDataComponent filterData)
        {
            return View(filterData);
        }
    }
}
