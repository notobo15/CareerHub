using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;
using System.Threading.Tasks;

namespace RecruitmentApp.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsFollowingAsync(string userId, int companyId)
        {
            return await _context.Followers
                .AnyAsync(f => f.UserId == userId && f.CompanyId == companyId);
        }
    }
}
