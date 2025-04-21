using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RecruitmentApp.Hubs
{
    public class MailHub : Hub
    {
        // Gửi thông báo đến user cụ thể (theo userId)
        public async Task NotifyNewMail(int userId)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveMailNotification");
        }
    }
}
