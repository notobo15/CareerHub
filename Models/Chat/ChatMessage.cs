using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models.Chat
{
    public class ChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RoomId { get; set; }
        public ChatRoom Room { get; set; }

        [Required]
        public string SenderId { get; set; }
        public AppUser Sender { get; set; }

        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

}
