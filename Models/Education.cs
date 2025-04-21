using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class Education
    {
        public int Id { get; set; }

        [Required]
        public string School { get; set; }

        [Required]
        public string Major { get; set; }

        public bool IsStudying { get; set; }

        [Required]
        public int FromMonth { get; set; }

        [Required]
        public int FromYear { get; set; }

        public int? ToMonth { get; set; }

        public int? ToYear { get; set; }

        public string? Details { get; set; }

        [Required]
        public string? UserId { get; set; }
        public AppUser User { get; set; }
    }
}
