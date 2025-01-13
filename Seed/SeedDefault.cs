using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using System;
using RecruitmentApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;

namespace RecruitmentApp.Seed
{
    public static class SeedDefault
    {
        public static async Task SeedAsync(AppDbContext applicationDbContext)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Seed", "seed.xlsx");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Excel file not found at path: {filePath}");
                return;
            }

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Seed Provinces
                    await SeedProvinces(applicationDbContext, package);

                    // Seed Districts
                    await SeedDistricts(applicationDbContext, package);

                    // Seed Wards
                    await SeedWards(applicationDbContext, package);

                    // Seed Companies
                    await SeedCompanies(applicationDbContext, package);

                    // Seed Levels
                    await SeedLevels(applicationDbContext, package);

                    await SeedSkills(applicationDbContext, package);

                    await SeedTitles(applicationDbContext, package);
                    
                    await SeedPosts(applicationDbContext, package);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }


        private static async Task SeedProvinces(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Provinces"];
            if (sheet != null && !applicationDbContext.Provinces.Any())
            {
                Console.WriteLine("Seeding Provinces...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var province = new Province
                    {
                        Code = sheet.Cells[row, 1].Text,
                        CodeName = sheet.Cells[row, 2].Text,
                        FullName = sheet.Cells[row, 3].Text,
                        FullNameEn = sheet.Cells[row, 4].Text,
                        Name = sheet.Cells[row, 5].Text,
                        NameEn = sheet.Cells[row, 6].Text
                    };

                    applicationDbContext.Provinces.Add(province);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Provinces seeded successfully.");
            }
        }

        private static async Task SeedDistricts(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Districts"];
            if (sheet != null && !applicationDbContext.Districts.Any())
            {
                Console.WriteLine("Seeding Districts...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var district = new District
                    {
                        Code = sheet.Cells[row, 1].Text,
                        Name = sheet.Cells[row, 2].Text,
                        NameEn = sheet.Cells[row, 3].Text,
                        FullName = sheet.Cells[row, 4].Text,
                        FullNameEn = sheet.Cells[row, 5].Text,
                        CodeName = sheet.Cells[row, 6].Text,
                        ProvinceId = sheet.Cells[row, 7].Text
                    };

                    applicationDbContext.Districts.Add(district);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Districts seeded successfully.");
            }
        }

        private static async Task SeedWards(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Wards"];
            if (sheet != null && !applicationDbContext.Wards.Any())
            {
                Console.WriteLine("Seeding Wards...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var ward = new Ward
                    {
                        Code = sheet.Cells[row, 1].Text,
                        Name = sheet.Cells[row, 2].Text,
                        NameEn = sheet.Cells[row, 3].Text,
                        FullName = sheet.Cells[row, 4].Text,
                        FullNameEn = sheet.Cells[row, 5].Text,
                        CodeName = sheet.Cells[row, 6].Text,
                        DistrictId = sheet.Cells[row, 7].Text
                    };

                    applicationDbContext.Wards.Add(ward);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Wards seeded successfully.");
            }
        }


        private static async Task SeedCompanies(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Companies"];
            if (sheet != null && !applicationDbContext.Companies.Any())
            {
                Console.WriteLine("Seeding Companies...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var company = new Company
                    {
                        // CompanyId = int.Parse(sheet.Cells[row, 1].Text), // Column 1: CompanyId
                        Slug = sheet.Cells[row, 2].Text, // Column 2: Slug
                        Name = sheet.Cells[row, 3].Text, // Column 3: Name
                        FullName = sheet.Cells[row, 4].Text, // Column 4: FullName
                        Size = sheet.Cells[row, 5].Text, // Column 5: Size

                        Description = sheet.Cells[row, 6].Text, // Column 6: Description
                        Reason = sheet.Cells[row, 7].Text, // Column 7: Reason
                        Phone = sheet.Cells[row, 8].Text, // Column 8: Phone
                        Email = sheet.Cells[row, 9].Text, // Column 9: Email
                        Type = sheet.Cells[row, 10].Text, // Column 10: Type
                        Nation = sheet.Cells[row, 11].Text, // Column 11: Nation
                        OverTime = sheet.Cells[row, 12].Text, // Column 12: OverTime
                        WorkingTime = sheet.Cells[row, 13].Text, // Column 13: WorkingTime
                        LogoImage = sheet.Cells[row, 14].Text, // Column 14: LogoImage
                        CompanyUrl = sheet.Cells[row, 15].Text, // Column 15: CompanyUrl
                        CompanyFbUrl = sheet.Cells[row, 16].Text, // Column 16: CompanyFbUrl
                        // RecruiterId = sheet.Cells[row, 17].Text, // Column 17: RecruiterId (nullable)
                        IsDeleted = sheet.Cells[row, 18].Text == "1", // Column 18: IsDeleted
                        CreatedAt = DateTime.Parse(sheet.Cells[row, 19].Text), // Column 19: CreatedAt
                        UpdatedAt = DateTime.Parse(sheet.Cells[row, 20].Text) // Column 20: UpdatedAt
                    };

                    applicationDbContext.Companies.Add(company);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Companies seeded successfully.");
            }
        }

        private static async Task SeedLevels(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Levels"];
            if (sheet != null && !applicationDbContext.Levels.Any())
            {
                Console.WriteLine("Seeding Levels...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++) // Bỏ qua hàng tiêu đề
                {
                    var level = new Level
                    {
                        // LevelId = int.Parse(sheet.Cells[row, 1].Text), // Cột 1: LevelId
                        Name = sheet.Cells[row, 2].Text // Cột 2: Name
                    };

                    applicationDbContext.Levels.Add(level);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Levels seeded successfully.");
            }
        }

        private static async Task SeedPosts(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Posts"]; // Tên sheet trong Excel
            if (sheet != null && !applicationDbContext.Posts.Any())
            {
                Console.WriteLine("Seeding Posts...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++) // Bỏ qua hàng tiêu đề
                {
                    var post = new Post
                    {
                        Title = sheet.Cells[row, 2].Text,
                        Slug = sheet.Cells[row, 3].Text,
                        IsHot = sheet.Cells[row, 4].Text == "1",
                        ViewTotal = int.TryParse(sheet.Cells[row, 5].Text, out var viewTotal) ? viewTotal : 0,
                        // Salary = sheet.Cells[row, 6].Text,
                        MinSalary = double.TryParse(sheet.Cells[row, 7].Text, out var minSalary) ? minSalary : 0,
                        MaxSalary = double.TryParse(sheet.Cells[row, 8].Text, out var maxSalary) ? maxSalary : 0,
                        PostDate = DateTime.Parse(sheet.Cells[row, 9].Text),
                        Expired = DateTime.Parse(sheet.Cells[row, 10].Text),
                        WorkSpace = sheet.Cells[row, 11].Text,
                        // AddressId = int.TryParse(sheet.Cells[row, 12].Text, out var addressId) ? (int?)addressId : null,
                        Description = sheet.Cells[row, 13].Text,
                        JobRequirement = sheet.Cells[row, 14].Text,
                        Benifit = sheet.Cells[row, 15].Text,
                        CompanyId = int.Parse(sheet.Cells[row, 16].Text),
                        // RecruiterId = sheet.Cells[row, 17].Text,
                        IsShow = sheet.Cells[row, 18].Text == "1",
                        IsClose = sheet.Cells[row, 19].Text == "1",
                        DegreeRequirement = sheet.Cells[row, 20].Text,
                        Quantity = int.TryParse(sheet.Cells[row, 21].Text, out var quantity) ? quantity : 0,
                        // IsDeleted = sheet.Cells[row, 22].Text == "1",
                        // CreatedAt = DateTime.Parse(sheet.Cells[row, 23].Text),
                        // UpdatedAt = DateTime.Parse(sheet.Cells[row, 24].Text)
                    };

                    applicationDbContext.Posts.Add(post);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Posts seeded successfully.");
            }
        }

        private static async Task SeedSkills(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Skills"];
            if (sheet != null && !applicationDbContext.Skills.Any())
            {
                Console.WriteLine("Seeding Skills...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++) // Bỏ qua hàng tiêu đề
                {
                    var skill = new Skill
                    {
                        // SkillId = int.Parse(sheet.Cells[row, 1].Text), // Cột 1: SkillId
                        Name = sheet.Cells[row, 2].Text, // Cột 2: Name
                        IsDeleted = sheet.Cells[row, 3].Text == "0", // Cột 3: IsDeleted (Chuyển đổi từ 0 hoặc 1)
                        CreatedAt = DateTime.Parse(sheet.Cells[row, 4].Text), // Cột 4: CreatedAt
                        UpdatedAt = string.IsNullOrWhiteSpace(sheet.Cells[row, 5].Text)
                            ? DateTime.MinValue
                            : DateTime.Parse(sheet.Cells[row, 5].Text) // Cột 5: UpdatedAt (Nếu trống thì để MinValue)
                    };

                    applicationDbContext.Skills.Add(skill);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Skills seeded successfully.");
            }
        }

        private static async Task SeedTitles(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Titles"];
            if (sheet != null && !applicationDbContext.Titles.Any())
            {
                Console.WriteLine("Seeding Titles...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++) // Bỏ qua hàng tiêu đề
                {
                    var title = new Title
                    {
                        // Id = int.Parse(sheet.Cells[row, 1].Text), // Cột 1: Id
                        Name = sheet.Cells[row, 2].Text // Cột 2: Name
                    };

                    applicationDbContext.Titles.Add(title);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Titles seeded successfully.");
            }
        }
    }
}