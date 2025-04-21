using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models.Email
{
    public class MailLabel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MailId { get; set; }
        public Mail Mail { get; set; }
        public int LabelId { get; set; }
        public Label Label { get; set; }
    }
}
