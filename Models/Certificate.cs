using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class Certificate
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Organization { get; set; }

        public int IssueMonth { get; set; }
        public int IssueYear { get; set; }

        public string? Url { get; set; }

        public string? Description { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
