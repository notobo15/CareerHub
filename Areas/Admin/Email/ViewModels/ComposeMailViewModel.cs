using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RecruitmentApp.Areas.Admin.Email.ViewModels
{
    public class ComposeMailViewModel
    {
        public List<string> ToUserIds { get; set; }
        public List<string> CcUserIds { get; set; } // Nếu bạn giữ lại cc
        public string Subject { get; set; }
        public string Body { get; set; }
        public IFormFileCollection Files { get; set; }

        public IEnumerable<SelectListItem> AllUsers { get; set; } // để load danh sách email
    }
}
