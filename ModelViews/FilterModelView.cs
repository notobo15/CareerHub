using System.Collections.Generic;

namespace RecruitmentApp.ModelViews
{
    public class FilterModelView
    {
        public List<string> jobLevels { set; get; } = new List<string>();
        public List<string> workingModels { set; get; } = new List<string>();
        public List<string> industries { set; get; } = new List<string>();
        public List<string> companyTypes { set; get; } = new List<string>();

        public string province { set; get; } = "0";
        public string key { set; get; } = "all";
        public int page { set; get; } = 1;
        public Salary salary { set; get; }

        public class Salary
        {
            public int min { set; get; } = 0;
            public int max { set; get; } = 0;
        }
    }

   
}
