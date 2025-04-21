using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class BlockCompanyInvitation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
