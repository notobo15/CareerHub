using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;

namespace RecruitmentApp.Services.Users
{
    public class CompanyService
    {
        private readonly AppDbContext _dbContext;

        public CompanyService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Company>> GetTopCompaniesAsync(int take)
        {
            return await _dbContext.Companies
                .Include(c => c.CompanySkills)
                    .ThenInclude(ck => ck.Skill)
                .Include(c => c.Locations)
                    .ThenInclude(l => l.Address)
                        .ThenInclude(a => a.City)
                .Include(c => c.Posts)
                .Distinct()
                .OrderBy(c => c.CompanyId) // đảm bảo thứ tự để phân trang chính xác
                .Take(take)
                .Where(c => c.IsShowOnHome)
                .ToListAsync();
        }
    }
}
