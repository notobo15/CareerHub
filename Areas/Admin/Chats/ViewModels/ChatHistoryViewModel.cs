using System;

namespace RecruitmentApp.Areas.Admin.Chats.ViewModels
{
    public class ChatHistoryViewModel
    {
        public int RoomId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string LastMessage { get; set; }
        public DateTime SentAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
