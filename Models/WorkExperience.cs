using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class WorkExperience
    {
        public int Id { get; set; }

        [Required]
        public string JobTitle { get; set; }

        [Required]
        public string Company { get; set; }

        public bool IsCurrentlyWorking { get; set; }

        [Required]
        public int FromMonth { get; set; }

        [Required]
        public int FromYear { get; set; }

        public int? ToMonth { get; set; }

        public int? ToYear { get; set; }

        public string? Description { get; set; }
        public string? Project { get; set; }

        [Required]
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
