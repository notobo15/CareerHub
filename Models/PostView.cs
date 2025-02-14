using System.ComponentModel.DataAnnotations;
using System;

namespace RecruitmentApp.Models
{
    public class PostView
    {
        [Key]
        public int Id { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string IpAddress { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.Now;
    }
}
