using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Areas.Admin.Email.Services
{
    public class EmailStatsService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EmailStatsService(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> GetInboxCountAsync(string userId)
        {
            return await _context.MailRecipients
                .Where(r => r.UserId == userId && !r.Mail.IsTrash)
                .CountAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.MailRecipients
                .Where(r => r.UserId == userId && !r.Mail.IsRead && !r.Mail.IsTrash)
                .CountAsync();
        }
    }
}
