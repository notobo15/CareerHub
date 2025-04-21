using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Follower
    {
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        public int? CompanyId { get; set; }
       [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
