using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class ApplyPost
    {
        public int PostID { get; set; }
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public AppUser User { get; set; }

        [ForeignKey("PostID")]
        public Post Post { get; set; }

        public string FileName { get; set; }
        public DateTime ApplyDate {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
