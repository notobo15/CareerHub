using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Data;
using RecruitmentApp.Models;
using RecruitmentApp.Services;

namespace RecruitmentApp.Areas.Admin.Database.Controllers
{
    [Area("Database")]
    [Route("/admin/database/[action]")]

    public class DbManageController : Controller

    {
        
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ExcelService _excelService;
        [TempData]
        public string StatusMessage { get; set; }

        public DbManageController(AppDbContext appDbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ExcelService excelService)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _excelService = excelService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        // [Authorize(Roles = RoleName.Administrator)]
        public IActionResult DeleteDb()
        {
            return View();
        }


        [HttpGet]
        //[Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> DeleteDbAsync()
        {


            var success = await _appDbContext.Database.EnsureDeletedAsync();

            StatusMessage = success ? "Xóa Database thành công" : "Không xóa được Db";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _appDbContext.Database.MigrateAsync();

            StatusMessage = "Cập nhật Database thành công";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult SeedDataDefault()
        {
            var sql = System.IO.File.ReadAllText("create-data-provices.sql");
            _appDbContext.Database.ExecuteSqlRaw(sql);
            StatusMessage = "create va data provice thanh cong";
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> SeedDataUser()
        {
            var rolesName = typeof(RoleName).GetFields().ToList();
            foreach (var item in rolesName)
            {
                var roleName = (string)item.GetRawConstantValue();
                var rFound = await _roleManager.FindByNameAsync(roleName);
                if (rFound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var userAdmin = await _userManager.FindByEmailAsync("chrisnguyeen2000@gmail.com");
            if (userAdmin == null)
            {
                userAdmin = new AppUser()
                {
                    UserName = "admin",
                    Email = "chrisnguyeen2000@gmail.com",
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(userAdmin, "123456");
                await _userManager.AddToRoleAsync(userAdmin, RoleName.Admin);
            }
           

        
            // POST
            var user = _userManager.GetUserAsync(this.User).Result;
            await _appDbContext.SaveChangesAsync();
            StatusMessage = "Send data success";
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> SeedDataTitle()
        {
            // CLEAR DATA
            _appDbContext.Titles.RemoveRange(_appDbContext.Titles.ToList());
            // Title
            List<Title> titles = new List<Title>() {
              new (){ Name = ".NET Developer",},
                new (){ Name = "Android",},
                new (){ Name = "Angular",},
                new (){ Name = "AngularJS",},
                new (){ Name = "ASP.NET",},
                new (){ Name = "Automation Test",},
                new (){ Name = "AWS",},
                new (){ Name = "Azure",},
                new (){ Name = "Blockchain",},
                new (){ Name = "Bridge Engineer",},
                new (){ Name = "Business Analyst",},
                new (){ Name = "C#",},
                new (){ Name = "C++",},
                new (){ Name = "C language",},
                new (){ Name = "Cloud",},
                new (){ Name = "CSS",},
                new (){ Name = "Dart",},
                new (){ Name = "Data Analyst",},
                new (){ Name = "Database",},
                new (){ Name = "Designer",},
                new (){ Name = "DevOps",},
                new (){ Name = "Django",},
                new (){ Name = "Elixir",},
                new (){ Name = "Embedded",},
                new (){ Name = "English",},
                new (){ Name = "ERP",},
                new (){ Name = "Flutter",},
                new (){ Name = "Games",},
                new (){ Name = "Golang",},
                new (){ Name = "HTML5",},
                new (){ Name = "iOS",},
                new (){ Name = "IT Support",},
                new (){ Name = "J2EE",},
                new (){ Name = "Japanese",},
                new (){ Name = "Java",},
                new (){ Name = "JavaScript",},
                new (){ Name = "JSON",},
                new (){ Name = "Kotlin",},
                new (){ Name = "Laravel",},
                new (){ Name = "Linux",},
                new (){ Name = "Magento",},
                new (){ Name = "Manager",},
                new (){ Name = "MVC",},
                new (){ Name = "MySQL",},
                new (){ Name = ".NET",},
                new (){ Name = "Networking",},
                new (){ Name = "NodeJS",},
                new (){ Name = "NoSQL",},
                new (){ Name = "Objective C",},
                new (){ Name = "OOP",},
                new (){ Name = "Oracle",},
                new (){ Name = "PHP",},
                new (){ Name = "PostgreSql",},
                new (){ Name = "Product Manager",},
                new (){ Name = "Project Manager",},
                new (){ Name = "Python",},
                new (){ Name = "QA QC",},
                new (){ Name = "ReactJS",},
                new (){ Name = "React Native",},
                new (){ Name = "Ruby",},
                new (){ Name = "Ruby on Rails",},
                new (){ Name = "SAP",},
                new (){ Name = "Scala",},
                new (){ Name = "Scrum",},
                new (){ Name = "Sharepoint",},
                new (){ Name = "Software Architect",},
                new (){ Name = "Solidity",},
                new (){ Name = "Spring",},
                new (){ Name = "SQL",},
                new (){ Name = "Swift",},
                new (){ Name = "System Admin",},
                new (){ Name = "System Engineer",},
                new (){ Name = "Team Leader",},
                new (){ Name = "Tester",},
                new (){ Name = "TypeScript",},
                new (){ Name = "UI-UX",},
                new (){ Name = "Unity",},
                new (){ Name = "VueJS",},
                new (){ Name = "Wordpress",},
                };
            await _appDbContext.Titles.AddRangeAsync(titles.ToList());
      
            await _appDbContext.SaveChangesAsync();
            StatusMessage = "Send data title thanh cong";
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> SeedDataLevel()
        {

            _appDbContext.Levels.RemoveRange(_appDbContext.Levels.ToList());
            // Level
            var levels = new List<Level>() {
                new ()
                {Name = "Intern",
                },
                 new (){ Name = "Fresher",},
                 new ()
                {
                    Name = "Junior",
                },
                   new ()
                {
                    Name = "Middle",
                },
                    new ()
                {
                    Name = "Senior",
                },
                     new()
                {
                    Name = "Principal",
                },

                };
            await _appDbContext.Levels.AddRangeAsync(levels.ToList());
            await _appDbContext.SaveChangesAsync();
            StatusMessage = "Send data title thanh cong";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SendDataSkill()
        {
            _appDbContext.Skills.RemoveRange(_appDbContext.Skills.ToList());

            // Skill
            List<Skill> skills = new List<Skill>() {
               new (){ Name = "Agile",},
                new (){ Name = "Android",},
                new (){ Name = "Angular",},
                new (){ Name = "AngularJS",},
                new (){ Name = "ASP.NET",},
                new (){ Name = "Automation Test",},
                new (){ Name = "AWS",},
                new (){ Name = "Azure",},
                new (){ Name = "Blockchain",},
                new (){ Name = "Bridge Engineer",},
                new (){ Name = "Business Analyst",},
                new (){ Name = "C#",},
                new (){ Name = "C++",},
                new (){ Name = "C language",},
                new (){ Name = "Cloud",},
                new (){ Name = "CSS",},
                new (){ Name = "Dart",},
                new (){ Name = "Data Analyst",},
                new (){ Name = "Database",},
                new (){ Name = "Designer",},
                new (){ Name = "DevOps",},
                new (){ Name = "Django",},
                new (){ Name = "Elixir",},
                new (){ Name = "Embedded",},
                new (){ Name = "English",},
                new (){ Name = "ERP",},
                new (){ Name = "Flutter",},
                new (){ Name = "Games",},
                new (){ Name = "Golang",},
                new (){ Name = "HTML5",},
                new (){ Name = "iOS",},
                new (){ Name = "IT Support",},
                new (){ Name = "J2EE",},
                new (){ Name = "Japanese",},
                new (){ Name = "Java",},
                new (){ Name = "JavaScript",},
                new (){ Name = "JSON",},
                new (){ Name = "Kotlin",},
                new (){ Name = "Laravel",},
                new (){ Name = "Linux",},
                new (){ Name = "Magento",},
                new (){ Name = "Manager",},
                new (){ Name = "MVC",},
                new (){ Name = "MySQL",},
                new (){ Name = ".NET",},
                new (){ Name = "Networking",},
                new (){ Name = "NodeJS",},
                new (){ Name = "NoSQL",},
                new (){ Name = "Objective C",},
                new (){ Name = "OOP",},
                new (){ Name = "Oracle",},
                new (){ Name = "PHP",},
                new (){ Name = "PostgreSql",},
                new (){ Name = "Product Manager",},
                new (){ Name = "Project Manager",},
                new (){ Name = "Python",},
                new (){ Name = "QA QC",},
                new (){ Name = "ReactJS",},
                new (){ Name = "React Native",},
                new (){ Name = "Ruby",},
                new (){ Name = "Ruby on Rails",},
                new (){ Name = "SAP",},
                new (){ Name = "Scala",},
                new (){ Name = "Scrum",},
                new (){ Name = "Sharepoint",},
                new (){ Name = "Software Architect",},
                new (){ Name = "Solidity",},
                new (){ Name = "Spring",},
                new (){ Name = "SQL",},
                new (){ Name = "Swift",},
                new (){ Name = "System Admin",},
                new (){ Name = "System Engineer",},
                new (){ Name = "Team Leader",},
                new (){ Name = "Tester",},
                new (){ Name = "TypeScript",},
                new (){ Name = "UI-UX",},
                new (){ Name = "Unity",},
                new (){ Name = "VueJS",},
                new (){ Name = "Wordpress",},


                };
            await _appDbContext.Skills.AddRangeAsync(skills.ToList());
            await _appDbContext.SaveChangesAsync();
            StatusMessage = "Send data title thanh cong";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> SendDataCompany()
        {
            _appDbContext.Companies.RemoveRange(_appDbContext.Companies.ToList());
            /*
               // Company
            Faker<Company> FakerCompany = new Faker<Company>();
            FakerCompany.RuleFor(p => p.Name, f => f.Lorem.Sentence(1, 3).Trim('.'));
            FakerCompany.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            FakerCompany.RuleFor(p => p.Size, f => "100 - 1000 người");
            FakerCompany.RuleFor(p => p.Phone, f => f.Phone.PhoneNumber("############"));
            FakerCompany.RuleFor(p => p.Email, (f, u) => f.Internet.Email(u.Name));
            FakerCompany.RuleFor(p => p.LogoImage, f => f.Internet.Avatar());
            FakerCompany.RuleFor(p => p.Nation, f => "Việt Nam");
            FakerCompany.RuleFor(p => p.WorkingTime, "Thứ 2 - Thứ 6");
            FakerCompany.RuleFor(p => p.OverTime, f => "Không");
            FakerCompany.RuleFor(p => p.Type, f => "Product");

              for (int i = 0; i < 50; i++)
            {
                var newCompany = FakerCompany.Generate();
                newCompany.Name = $"Cong ty {i}";
                _appDbContext.Companies.Add(newCompany);
            }
             */
            var companies = _excelService.ReadExcelAndAddToDatabase();


            
            await _appDbContext.Companies.AddRangeAsync(companies);
            // Do something with the list of companies if needed
            await _appDbContext.SaveChangesAsync();
            StatusMessage = "Send data title thanh cong";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SeedDataPost()
        {
            _appDbContext.Posts.RemoveRange(_appDbContext.Posts.ToList());
            var posts = _excelService.ReadExcelPostAndAddToDatabase();
            /*
          Faker<Post> FakerPosts = new Faker<Post>();
          FakerPosts.RuleFor(p => p.Title, f => f.Lorem.Sentence(3, 4).Trim('.'));
          FakerPosts.RuleFor(p => p.Benifit, f => "Benifit");
          FakerPosts.RuleFor(p => p.IsHot, f => false);
          FakerPosts.RuleFor(p => p.Salary, f => "1000 - 2000$");
          FakerPosts.RuleFor(p => p.WorkSpace, f => "Văn Phòng");
          FakerPosts.RuleFor(p => p.PostDate, f => f.Date.Between(new DateTime(2023, 01, 01), DateTime.Now));
          FakerPosts.RuleFor(p => p.AppUser, user);
          for (int i = 0; i < 50; i++)
          {
              var newPost = FakerPosts.Generate();
              _appDbContext.Posts.Add(newPost);
          }
            */
            await _appDbContext.Posts.AddRangeAsync(posts);
            await _appDbContext.SaveChangesAsync();
            StatusMessage = "Send data posts thanh cong";
            return RedirectToAction("Index");
        }
    }

}