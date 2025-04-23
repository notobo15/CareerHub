using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models.Chat
{
    public class ChatUserRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int RoomId { get; set; }
        public ChatRoom Room { get; set; }
    }
}
