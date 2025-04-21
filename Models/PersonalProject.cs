using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class PersonalProject
    {
        public int Id { get; set; }

        [Required]
        public string ProjectName { get; set; }

        public bool IsOngoing { get; set; }

        [Required]
        public int StartMonth { get; set; }

        [Required]
        public int StartYear { get; set; }

        public int? EndMonth { get; set; }

        public int? EndYear { get; set; }

        public string? Description { get; set; }
        public string? UrlProject { get; set; }

        [Required]
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
