using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.DTOs;
using RecruitmentApp.Models;
using RecruitmentApp.Utilities;

namespace RecruitmentApp.Services.Users;

public class PostService
{
    private readonly AppDbContext _dbContext;

    public PostService(AppDbContext context)
    {
        _dbContext = context;
    }
    public PostDTO GetById(int postId)
    {
        var p = _dbContext.Posts
                    .Include(p => p.PostSkills)
                    .ThenInclude(pk => pk.Skill)

                    .Include(p => p.Company)
                    .ThenInclude(cc => cc.Country)

                    .Include(p => p.Company)
                    .ThenInclude(cc => cc.CompanyIndustries)
                    .ThenInclude(ci => ci.Industry)

                    .Include(p => p.PostLocations)
                    .ThenInclude(pl => pl.Location)
                    .ThenInclude(pla => pla.Address)
                    .ThenInclude(pla => pla.City)

                    .Where(p => p.PostId == postId).FirstOrDefault();
        // Giả sử bạn có service để lấy post
        if (p == null)
        {
            return null; // Trả về một thông báo thay vì lỗi
        }
        var postDTOs = new PostDTO
        {
            PostId = p.PostId,
            Title = p.Title,
            Slug = p.Slug,
            IsHot = p.IsHot,
            ViewTotal = p.ViewTotal,
            Salary = p.Salary,
            SalaryText = p.salaryToString(),
            PostDate = p.PostDate,
            Expired = p.Expired,
            WorkSpace = p.WorkSpace,
            TimeAgo = TimeFormatter.GetTimeAgo(p.PostDate),
            Description = p.Description,
            Benefit = p.Benefit,
            DegreeRequirement = p.DegreeRequirement,
            JobRequirement = p.JobRequirement,
            TopReason = p.TopReason,
            Company = p.Company != null ? new CompanyDTO
            {
                CompanyId = p.Company.CompanyId,
                Name = p.Company.Name,
                Slug = p.Company.Slug,
                Size = p.Company.Size,
                Description = p.Company.Description,
                Phone = p.Company.Phone,
                Email = p.Company.Email,
                WorkingTime = p.Company.WorkingTime,
                LogoImage = p.Company.LogoImage,
                Country = p.Company.Country.Name,
                CountryCode = p.Company.Country.ISOCode,
                ShortDescription = p.Company.ShortDescription,
                CompanyUrl = p.Company.CompanyUrl,
                Skills = p.Company.CompanySkills?.Select(cs => cs.Skill.Name).ToList(),
                Locations = p.Company.Locations?.Select(l => l.Address.City.Name).ToList(),
                Industries = p.Company.CompanyIndustries.Select(ps => new IndustryDTO
                {
                    IndustryId = ps.Industry.IndustryId,
                    Name = ps.Industry.Name,
                }).ToList(),
            } : null,
            Skills = p.PostSkills.Select(ps => new SkillDTO
            {
                SkillId = ps.Skill.SkillId,
                Name = ps.Skill.Name,
                Slug = ps.Skill.Slug
            }).ToList(),

            Addresses = p.PostLocations != null ? p.PostLocations.Select(pl => new AddressDTO
            {
                AddressId = pl.Location.Address.AddressId,
                Nation = pl.Location.Address.City?.Name,
                ProvinceCode = pl.Location.Address.City?.CodeName,
                Province = pl.Location.Address.City?.FullName,
                DetailPosition = pl.Location.Address.DetailPosition,
                GgMapSrc = pl.Location.Address.GgMapSrc,
                FullAddress = pl.Location.Address.FullAddress
            }).ToList() : new List<AddressDTO>()
        };

        return postDTOs;
    }

    public async Task<List<Post>> GetLatestPostsAsync(int take)
    {
        return await _dbContext.Posts
            .Include(p => p.PostSkills)
                .ThenInclude(pk => pk.Skill)
            .Include(p => p.Company)
            .Include(p => p.PostLocations)
                .ThenInclude(pl => pl.Location)
                    .ThenInclude(pla => pla.Address)
                        .ThenInclude(pla => pla.City)
            .OrderByDescending(p => p.PostDate)
            .Take(take)
            .ToListAsync();
    }
}
