using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class UserIndustry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int IndustryId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [ForeignKey("IndustryId")]
        public Industry Industry { get; set; }
    }
}
