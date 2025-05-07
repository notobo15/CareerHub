using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using System;
using RecruitmentApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using RecruitmentApp.Utilities;

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

                    await SeedCountries(applicationDbContext, package);

                    // Seed Companies
                    await SeedCompanies(applicationDbContext, package);

                    // Seed Levels
                    await SeedLevels(applicationDbContext, package);

                    await SeedSkills(applicationDbContext, package);

                    await SeedTitles(applicationDbContext, package);

                    await SeedAddresses(applicationDbContext, package);

                    await SeedLocations(applicationDbContext, package);

                    await SeedPosts(applicationDbContext, package);

                    await SeedCompanySkills(applicationDbContext, package);

                    await SeedPostSkills(applicationDbContext, package);

                    await SeedPostLocations(applicationDbContext, package);

                    await SeedImages(applicationDbContext, package);

                    await SeedIndustries(applicationDbContext, package);

                    await SeedCompanyIndustries(applicationDbContext, package);

                    await SeedWorkTypes(applicationDbContext, package);

                    await SeedPostWorkTypes(applicationDbContext, package);

                    await SeedCompanyTypes(applicationDbContext, package);

                    await SeedBlogCategories(applicationDbContext, package); 
                    
                    await SeedBlogs(applicationDbContext, package);

                    await SeedSettings(applicationDbContext);
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
                        NameEn = sheet.Cells[row, 6].Text,
                        Slug = AppUtilities.GenerateSlug(sheet.Cells[row, 5].Text)
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
                        //CompanyId = int.Parse(sheet.Cells[row, 1].Text), // Column 1: CompanyId
                        Slug = Utilities.AppUtilities.GenerateSlug(sheet.Cells[row, 3].Value?.ToString()),
                        //Slug = sheet.Cells[row, 2].Text, // Column 2: Slug
                        FullName = sheet.Cells[row, 3].Text, // Column 4: FullName
                        Name = sheet.Cells[row, 4].Text, // Column 3: Name
                        Size = sheet.Cells[row, 5].Text, // Column 5: Size
                        Description = sheet.Cells[row, 6].Text, // Column 6: Description
                        //Description = sheet.Cells[row, 6].Text, // Column 6: Description
                        Reason = sheet.Cells[row, 8].Text, // Column 7: Reason
                        Phone = sheet.Cells[row, 9].Text, // Column 8: Phone
                        Email = sheet.Cells[row, 10].Text, // Column 9: Email
                        Type = sheet.Cells[row, 11].Text, // Column 10: Type
                        Nation = sheet.Cells[row, 12].Text, // Column 11: Nation
                        OverTime = sheet.Cells[row, 13].Text, // Column 12: OverTime
                        WorkingTime = sheet.Cells[row, 14].Text, // Column 13: WorkingTime
                        LogoImage = sheet.Cells[row, 15].Text, // Column 14: LogoImage
                        LogoFullPath = "/images/companies/" + sheet.Cells[row, 15].Text, // Column 14: LogoImage
                        CompanyUrl = sheet.Cells[row, 16].Text, // Column 15: CompanyUrl
                        CompanyFbUrl = sheet.Cells[row, 17].Text, // Column 16: CompanyFbUrl
                        TopReason = sheet.Cells[row, 19].Text,
                        OurExpertise = sheet.Cells[row, 20].Text,
                        WhyJoinUs = sheet.Cells[row, 21].Text,
                        ShortDescription = sheet.Cells[row, 22].Text,
                        CountryId = int.Parse(sheet.Cells[row, 23].Text),
                        // RecruiterId = sheet.Cells[row, 17].Text, // Column 17: RecruiterId (nullable)
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsShowCompanyFbUrl = true,
                        IsShowCompanyUrl = true,
                        IsShowOnHome = (row - 2) < 8,
                        //IsDeleted = sheet.Cells[row, 18].Text == "1", // Column 18: IsDeleted
                        //CreatedAt = DateTime.Parse(sheet.Cells[row, 19].Text), // Column 19: CreatedAt
                        //UpdatedAt = DateTime.Parse(sheet.Cells[row, 20].Text), // Column 20: UpdatedAt

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
                        Name = sheet.Cells[row, 2].Text, // Cột 2: Name
                        Slug = AppUtilities.GenerateSlug(sheet.Cells[row, 2].Text)
                    };

                    applicationDbContext.Levels.Add(level);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Levels seeded successfully.");
            }
        }

        private static async Task SeedPosts(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Posts"];
            if (sheet != null && !applicationDbContext.Posts.Any())
            {
                Console.WriteLine("Seeding Posts...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var post = new Post
                    {
                        Title = sheet.Cells[row, 2].Text,
                        //Slug = sheet.Cells[row, 3].Text,
                        Slug = Utilities.AppUtilities.GenerateSlug(sheet.Cells[row, 2].Value?.ToString()),
                        IsHot = sheet.Cells[row, 4].Text == "1",
                        ViewTotal = int.TryParse(sheet.Cells[row, 5].Text, out var viewTotal) ? viewTotal : 0,
                        SalaryType = sheet.Cells[row, 6].Text,
                        MinSalary = double.TryParse(sheet.Cells[row, 7].Text, out var minSalary) ? minSalary : 0,
                        MaxSalary = double.TryParse(sheet.Cells[row, 8].Text, out var maxSalary) ? maxSalary : 0,
                        PostDate = DateTime.Parse(sheet.Cells[row, 9].Text),
                        Expired = DateTime.Parse(sheet.Cells[row, 10].Text),
                        WorkSpace = sheet.Cells[row, 11].Text,
                        // AddressId = int.TryParse(sheet.Cells[row, 12].Text, out var addressId) ? (int?)addressId : null,
                        TopReason = sheet.Cells[row, 12].Text,
                        Description = sheet.Cells[row, 13].Text,
                        JobRequirement = sheet.Cells[row, 14].Text,
                        Benefit = sheet.Cells[row, 15].Text,
                        CompanyId = int.Parse(sheet.Cells[row, 16].Text),
                        //LocationId = int.Parse(sheet.Cells[row, 22].Text),
                        // RecruiterId = sheet.Cells[row, 17].Text,
                        IsShow = sheet.Cells[row, 18].Text == "1",
                        IsClose = sheet.Cells[row, 19].Text == "1",
                        //DegreeRequirement = sheet.Cells[row, 20].Text,
                        Quantity = int.TryParse(sheet.Cells[row, 21].Text, out var quantity) ? quantity : 0,
                        // IsDeleted = sheet.Cells[row, 22].Text == "1",
                        // CreatedAt = DateTime.Parse(sheet.Cells[row, 23].Text),
                        // UpdatedAt = DateTime.Parse(sheet.Cells[row, 24].Text)
                        IsShowOnHome = (row - 2) < 8,
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
                        Name = sheet.Cells[row, 2].Text,
                        Slug = AppUtilities.GenerateSlug(sheet.Cells[row, 2].Text),
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
                        Name = sheet.Cells[row, 2].Text, // Cột 2: Name
                        Slug = AppUtilities.GenerateSlug(sheet.Cells[row, 2].Text)
                    };

                    applicationDbContext.Titles.Add(title);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Titles seeded successfully.");
            }
        }

        private static async Task SeedCompanySkills(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["CompanySkills"];
            if (sheet != null && !applicationDbContext.CompanySkills.Any())
            {
                Console.WriteLine("Seeding Company Skills...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var title = new CompanySkills
                    {
                        CompanyID = int.Parse(sheet.Cells[row, 2].Text),
                        SkillID = int.Parse(sheet.Cells[row, 3].Text)
                    };

                    applicationDbContext.CompanySkills.Add(title);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Company Skills seeded successfully.");
            }
        }

        private static async Task SeedPostSkills(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["PostSkills"];
            if (sheet != null && !applicationDbContext.PostSkills.Any())
            {
                Console.WriteLine("Seeding Post Skills...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var pk = new PostSkills
                    {
                        PostID = int.Parse(sheet.Cells[row, 2].Text),
                        SkillID = int.Parse(sheet.Cells[row, 3].Text)
                    };

                    applicationDbContext.PostSkills.Add(pk);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Post Skills seeded successfully.");
            }
        }

        private static async Task SeedPostLocations(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["PostLocations"];
            if (sheet != null && !applicationDbContext.PostLocations.Any())
            {
                Console.WriteLine("Seeding Post Locations...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var pk = new PostLocations
                    {
                        PostID = int.Parse(sheet.Cells[row, 2].Text),
                        LocationId = int.Parse(sheet.Cells[row, 3].Text)
                    };

                    applicationDbContext.PostLocations.Add(pk);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Post Skills seeded successfully.");
            }
        }

        private static async Task SeedAddresses(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Addresses"];
            if (sheet != null && !applicationDbContext.Addresses.Any())
            {
                Console.WriteLine("Seeding Addresses...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var address = new Address
                    {
                        ProvinceCode = sheet.Cells[row, 2].Text,
                        DistrictCode = sheet.Cells[row, 3].Text,
                        WardCode = sheet.Cells[row, 4].Text,
                        DetailPosition = sheet.Cells[row, 5].Text,
                        GgMapSrc = sheet.Cells[row, 6].Text,
                        FullAddress = sheet.Cells[row, 7].Text,
                    };

                    applicationDbContext.Addresses.Add(address);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Addresses seeded successfully.");
            }
        }

        private static async Task SeedLocations(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Locations"];
            if (sheet != null && !applicationDbContext.Locations.Any())
            {
                Console.WriteLine("Seeding Locations...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var location = new Location
                    {
                        AddressId = int.Parse(sheet.Cells[row, 2].Text),
                        CompanyId = int.Parse(sheet.Cells[row, 3].Text)
                    };

                    applicationDbContext.Locations.Add(location);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Locations seeded successfully.");
            }
        }
        private static async Task SeedImages(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Images"];
            if (sheet != null && !applicationDbContext.Images.Any())
            {
                Console.WriteLine("Seeding Images...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var image = new Image
                    {
                        CompanyId = int.Parse(sheet.Cells[row, 2].Text),
                        FileName = sheet.Cells[row, 3].Text,
                        FullPath = "/images/sliders/" + sheet.Cells[row, 3].Text

                    };

                    applicationDbContext.Images.Add(image);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Images seeded successfully.");
            }
        }

        private static async Task SeedIndustries(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Industries"];
            if (sheet != null && !applicationDbContext.Industries.Any())
            {
                Console.WriteLine("Seeding Industries...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var image = new Industry
                    {
                        Name = sheet.Cells[row, 2].Text
                    };

                    applicationDbContext.Industries.Add(image);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Industries seeded successfully.");
            }
        }


        private static async Task SeedCompanyIndustries(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["CompanyIndustries"];
            if (sheet != null && !applicationDbContext.CompanyIndustries.Any())
            {
                Console.WriteLine("Seeding Industries...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var image = new CompanyIndustry
                    {
                        CompanyId = int.Parse(sheet.Cells[row, 2].Text),
                        IndustryId = int.Parse(sheet.Cells[row, 3].Text),

                    };

                    applicationDbContext.CompanyIndustries.Add(image);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Industries seeded successfully.");
            }
        }

        private static async Task SeedWorkTypes(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["WorkTypes"];
            if (sheet != null && !applicationDbContext.WorkTypes.Any())
            {
                Console.WriteLine("Seeding WorkTypes...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var workType = new WorkType
                    {
                        Name = sheet.Cells[row, 2].Text,
                        Slug = AppUtilities.GenerateSlug(sheet.Cells[row, 2].Text),

                    };

                    applicationDbContext.WorkTypes.Add(workType);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("WorkTypes seeded successfully.");
            }
        }
        private static async Task SeedPostWorkTypes(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["PostWorkTypes"];
            if (sheet != null && !applicationDbContext.PostWorkTypes.Any())
            {
                Console.WriteLine("Seeding PostWorkTypes...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var postWorkType = new PostWorkType
                    {
                        PostId = int.Parse(sheet.Cells[row, 2].Text),
                        WorkTypeId = int.Parse(sheet.Cells[row, 3].Text),

                    };

                    applicationDbContext.PostWorkTypes.Add(postWorkType);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("PostWorkTypes seeded successfully.");
            }
        }
        private static async Task SeedCountries(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Countries"];
            if (sheet != null && !applicationDbContext.CompanyIndustries.Any())
            {
                Console.WriteLine("Seeding Countries...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var country = new Country
                    {
                        Slug = AppUtilities.GenerateSlug(sheet.Cells[row, 2].Text),
                        Name = sheet.Cells[row, 2].Text,
                        ISOCode = sheet.Cells[row, 3].Text,

                    };

                    applicationDbContext.Countries.Add(country);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("Countries seeded successfully.");
            }
        }

        private static async Task SeedCompanyTypes(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["CompanyTypes"];
            if (sheet != null && !applicationDbContext.CompanyTypes.Any())
            {
                Console.WriteLine("Seeding CompanyTypes...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var companyType = new CompanyType
                    {
                        Slug = AppUtilities.GenerateSlug(sheet.Cells[row, 2].Text),
                        Name = sheet.Cells[row, 2].Text,

                    };

                    applicationDbContext.CompanyTypes.Add(companyType);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("CompanyTypes seeded successfully.");
            }
        }
        private static async Task SeedBlogCategories(AppDbContext applicationDbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["BlogCategories"];
            if (sheet != null && !applicationDbContext.BlogCategories.Any())
            {
                Console.WriteLine("Seeding BlogCategories...");
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var blogCategory = new BlogCategory
                    {
                        Slug = AppUtilities.GenerateSlug(sheet.Cells[row, 2].Text),
                        Name = sheet.Cells[row, 2].Text,
                        Description = sheet.Cells[row, 3].Text,

                    };

                    applicationDbContext.BlogCategories.Add(blogCategory);
                }

                await applicationDbContext.SaveChangesAsync();
                Console.WriteLine("BlogCategories seeded successfully.");
            }
        }

        private static async Task SeedBlogs(AppDbContext dbContext, ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets["Blogs"];
            if (sheet != null && !dbContext.Blogs.Any())
            {
                Console.WriteLine("Seeding Blogs...");

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    var title = sheet.Cells[row, 1].Text;
                    var description = sheet.Cells[row, 2].Text;
                    var content = sheet.Cells[row, 3].Text;
                    var thumbnail = sheet.Cells[row, 4].Text;
                    var categoryId = int.Parse(sheet.Cells[row, 5].Text);

                    // Tìm category nếu có
                    var category = dbContext.BlogCategories.FirstOrDefault(c => c.CategoryId == categoryId);

                    var blog = new Blog
                    {
                        Title = title,
                        Slug = AppUtilities.GenerateSlug(title),
                        Description = description,
                        Content = content,
                        ThumbnailUrl = thumbnail,
                        IsPublished = true,
                        CreatedAt = DateTime.Now,
                        CategoryId = category?.CategoryId,
                        IsShowOnHome = (row - 0) < 5,
                    };

                    dbContext.Blogs.Add(blog);
                }

                await dbContext.SaveChangesAsync();
                Console.WriteLine("Blogs seeded successfully.");
            }
        }
        private static async Task SeedSettings(AppDbContext dbContext)
        {
            if (!dbContext.Settings.Any())
            {
                Console.WriteLine("Seeding Settings...");

                var settings = new List<Setting>
        {
            new Setting
            {
                NumberOfPosts = 6,
                NumberOfCompanies = 10,
                PhoneNumber = "0123456789",
                Email = "admin@example.com",
                TaxNumber = "1234567890"
            }
        };

                dbContext.Settings.AddRange(settings);
                await dbContext.SaveChangesAsync();

                Console.WriteLine("Settings seeded successfully.");
            }
        }
    }
}