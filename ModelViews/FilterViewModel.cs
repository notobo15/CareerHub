using System.Collections.Generic;
using RecruitmentApp.Models;

namespace RecruitmentApp.ModelViews
{
    public class FilterViewModel
    {
        public List<Industry> Industries { get; set; }
        public List<Level> Levels { get; set; }

        public List<WorkType> WorkTypes { get; set; }
        public List<CompanyType> CompanyTypes { get; set; }
    }
}
