using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace RecruitmentApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task SendMessage(string roomId, string senderId, string message)
        {
            await Clients.Group(roomId).SendAsync("ReceiveMessage", senderId, message, DateTime.UtcNow);
        }
    }
}
