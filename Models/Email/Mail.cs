using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models.Email
{
    public class Mail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;

        public string SenderId { get; set; }
        public AppUser Sender { get; set; }

        public int? ReplyToMailId { get; set; }
        [ForeignKey("ReplyToMailId")]
        public Mail ReplyToMail { get; set; }


        public List<MailRecipient> Recipients { get; set; }
        public List<Attachment> Attachments { get; set; }
        public List<MailLabel> MailLabels { get; set; }

        public bool IsRead { get; set; } = false;
        public bool IsImportant { get; set; } = false;
        public bool IsStarred { get; set; } = false;
        public bool IsSpam { get; set; } = false;
        public bool IsTrash { get; set; } = false;
        public bool IsDraft { get; set; } = false;
    }

}
