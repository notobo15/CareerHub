using RecruitmentApp.Models;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using Microsoft.CodeAnalysis.Host;
using System;

namespace RecruitmentApp.Services
{
    public class ExcelService
    {
        public List<Company> ReadExcelAndAddToDatabase()
        {
            List<Company> companies = new List<Company>();
            var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "data.xlsx");


            using (var package = new ExcelPackage(excelFilePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    // Assuming your Excel columns are in the order:
                    // Slug, Name, Size, Description, Reason, Phone, Email, Type, Nation, OverTime, WorkingTime, LogoImage, CompanyUrl, CompanyFbUrl, UserId
                    var company = new Company
                    {
                        // CompanyId = Convert.ToInt32(worksheet.Cells[row, 1].Value?.ToString()),

                        FullName = worksheet.Cells[row, 2].Value?.ToString(),

                        Name = worksheet.Cells[row, 3].Value?.ToString(),

                        Slug = Utilities.AppUtilities.GenerateSlug(worksheet.Cells[row, 2].Value?.ToString()),

                        Type = worksheet.Cells[row, 4].Value?.ToString(),

                        Size = worksheet.Cells[row, 5].Value?.ToString(),

                        Nation = worksheet.Cells[row, 6].Value?.ToString(),

                        OverTime = worksheet.Cells[row, 7].Value?.ToString(),

                        WorkingTime = worksheet.Cells[row, 8].Value?.ToString(),

                        Description = worksheet.Cells[row, 9].Value?.ToString(),

                        LogoImage = worksheet.Cells[row, 10].Value?.ToString(),

                        CompanyUrl = worksheet.Cells[row, 11].Value?.ToString(),

                        CompanyFbUrl = worksheet.Cells[row, 12].Value?.ToString(),

                        Reason = worksheet.Cells[row, 13].Value?.ToString(),

                        Phone = worksheet.Cells[row, 14].Value?.ToString(),

                        Email = worksheet.Cells[row, 15].Value?.ToString(),

                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,

                        // UserId = worksheet.Cells[row, 15].Value?.ToString()
                    };

                    companies.Add(company);
                }
            }

            // Add the list of companies to the database
            // YourDatabaseContext.Companies.AddRange(companies);
            // YourDatabaseContext.SaveChanges();

            return companies;

        }
            public List<Post> ReadExcelPostAndAddToDatabase()
            {
                List<Post> list = new List<Post>();
                var excelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "post.xlsx");


                using (var package = new ExcelPackage(excelFilePath))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        Post post = new Post
                        {
                            // PostId = Convert.ToInt32(worksheet.Cells[row, 1].Value.ToString()),
                            Title = worksheet.Cells[row,2].Value?.ToString(),

                            Slug = Utilities.AppUtilities.GenerateSlug(worksheet.Cells[row, 2].Value?.ToString()),

                            MinSalary = Convert.ToDouble(worksheet.Cells[row, 3].Value.ToString()),

                            MaxSalary = Convert.ToDouble(worksheet.Cells[row, 4].Value.ToString()),

                           CompanyId = Convert.ToInt32(worksheet.Cells[row, 5].Value?.ToString()),

                            WorkSpace = worksheet.Cells[row, 6].Value?.ToString(),

                            JobRequirement = worksheet.Cells[row,7].Value?.ToString(),

                            Benifit = worksheet.Cells[row,8].Value?.ToString(),

                            DegreeRequirement = worksheet.Cells[row,9].Value?.ToString(),

                            Quantity = Convert.ToInt32(worksheet.Cells[row, 10].Value?.ToString()),

                            Expired = DateTime.Now,

                            PostDate = DateTime.Now,

                            IsShow = true,

                            IsClose = false,

                            IsDeleted = false,

                            CreatedAt = DateTime.Now,

                            UpdatedAt = DateTime.Now,
                        };

                        list.Add(post);
                    }
                }

                return list;
            }
        }
    } 
