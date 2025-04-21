using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models.Email
{
    public class MailRecipient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MailId { get; set; }
        public Mail Mail { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
