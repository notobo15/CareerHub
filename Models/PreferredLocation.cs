using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class PreferredLocation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Code { get; set; }
        [ForeignKey("Code")]
        public Province Province { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
