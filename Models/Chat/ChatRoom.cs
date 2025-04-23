using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models.Chat
{
    public class ChatRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string RoomName { get; set; }  // có thể để null nếu dùng 1:1

        public ICollection<ChatMessage> Messages { get; set; }
        public ICollection<ChatUserRoom> Participants { get; set; }
    }

}
