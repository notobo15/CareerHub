using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class BlogView
    {
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int BlogId { get; set; }
        public Blog Blog { get; set; }

        public string? UserId { get; set; }
        public AppUser? User { get; set; }

        public string IpAddress { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    }
}
