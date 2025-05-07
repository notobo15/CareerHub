using System;

namespace RecruitmentApp.Areas.Recruiter.Users.ViewModels
{
    public class CVFilterResult
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FilePath { get; set; }
        public DateTime ApplyDate { get; set; }

        public bool IsFit { get; set; }
        public string Reason { get; set; }
    }
}
