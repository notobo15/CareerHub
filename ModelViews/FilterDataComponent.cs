using System.Collections.Generic;
using RecruitmentApp.DTOs;

namespace RecruitmentApp.ModelViews
{
    public class FilterDataComponent
    {
        public List<PostDTO> posts { get; set; }
        public PostDTO post { get; set; }

        public int currentPage { get; set; }
        public int totalPages { get; set; }

        public string key { set; get; }
        public string province { set; get; }
        public int totalPosts { set; get; }
    }
}
