using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace RecruitmentApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment env;
        [TempData]
        public string StatusMessage { get; set; }

        /*
        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;


        }
        */
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            env = environment;


        }



        // public IActionResult Index()
        // {
        //  var companies = _appDbContext.Companies
        //      .Include(c => c.CompanySkills)
        //      .ThenInclude(ck => ck.Skill)
        //      .Include(c => c.Posts)
        //      .Include(c => c.Addresses)
        //      .OrderBy(c => c.CompanyId)
        //    .Take(5)
        //    .AsSplitQuery()
        //    .ToList();
        // var provices = _appDbContext.Provinces.ToList();
        // var topProvinces = _appDbContext.Provinces.Where(p => p.Code == "01" || p.Code == "48" || p.Code == "79").ToList();
        //  provices.RemoveAll(p => topProvinces.Contains(p));
        //  provices.InsertRange(0, topProvinces);
        //  ViewData["provices"] = provices;
        //  var posts = _appDbContext.Posts
        //      .Include(p => p.Company)
        //      .Include(p => p.Address)
        //      .ThenInclude(e => e.City)
        //      .Include(p => p.PostSkills)
        //      .OrderBy(c => c.PostId)
        //      .Take(8)
        //      
        //      .AsSplitQuery()
        //  .ToList();
        //
        //
        //   ViewData["levels"] = _appDbContext.Levels
        //          .ToList();
        //
        //  var totalPosts = _appDbContext.Posts.Count();
        //  ViewData["totalPosts"] = totalPosts;
        //
        //  ViewData["posts"] = posts;
        //
        //  ViewData["companies"] = companies;


        //     return View();

        // }

        // public IActionResult Privacy()
        // {
        //     return View();
        // }

        // public IActionResult AboutMe()
        // {
        //     return View();
        // }

        // public IActionResult ViewCompany(string id)
        // {
        //     if (id == null)
        //         return NotFound();
        //     var company = _appDbContext.Companies
        //         .Include(c => c.Addresses)
        //         .ThenInclude(e => e.City)
        //         .ThenInclude(e => e.Districts)
        //         .ThenInclude(e => e.Wards)
        //         .Include(c => c.Posts)
        //         .ThenInclude(p => p.PostSkills)
        //         .ThenInclude(e => e.Skill)
        //         .Include(c => c.CompanySkills)
        //         .ThenInclude(c => c.Skill)
        //         .AsSplitQuery()
        //         .FirstOrDefault(c => c.Slug == id);
        //     return View(company);
        // }

        // public IActionResult ViewPost(int id)
        // {
        //     var post = _appDbContext.Posts


        //         .Include(p => p.Company)
        //         .Include(p => p.PostSkills)
        //         .ThenInclude(e => e.Skill)

        //         .Include(e => e.PostLevels)
        //         .ThenInclude(e => e.Level)

        //         .Include(p => p.Address)

        //         .AsSplitQuery()
        //         .FirstOrDefault(p => p.PostId == id);

        //     post.Address = _appDbContext.Addresses.FirstOrDefault(a => a.AddressId == post.AddressId);
        //     post.ViewTotal++;
        //     _appDbContext.SaveChanges();

        //     return View(post);
        // }
        // [Authorize]
        // public IActionResult ApplyJob(string id)
        // {
        //     var post = _appDbContext.Posts.FirstOrDefault(e => e.Slug == id);
        //     if (post == null)
        //         return NotFound();


        //     return View(post);
        // }
        // [HttpPost]
        // public IActionResult ApplyCV(string userId, string id, string Name, IFormFile file, string Description)
        // {
        //     var maxFileSize = 3;
        //     var post = _appDbContext.Posts.FirstOrDefault(e => e.Slug == id);

        //     if (file != null)
        //     {

        //         // 1024 * 1024 ~ 1 MB
        //         if (file.Length > (maxFileSize * 1024 * 1024))
        //         {
        //             StatusMessage = "Filesize of image is too large. Maximum file size permitted is " + maxFileSize + "KB";
        //             return Redirect(Request.Headers["Referer"].ToString());
        //         }
        //         string ext = Path.GetExtension(file.FileName);
        //         string[] acceptedFileTypes = new string[7];
        //         acceptedFileTypes[0] = ".pdf";
        //         acceptedFileTypes[1] = ".doc";
        //         acceptedFileTypes[2] = ".docx";
        //         acceptedFileTypes[3] = ".jpg";
        //         acceptedFileTypes[4] = ".jpeg";
        //         acceptedFileTypes[5] = ".gif";
        //         acceptedFileTypes[6] = ".json";

        //         bool IsAccceptExt = false;
        //         //should we accept the file?
        //         for (int i = 0; i <= 6; i++)
        //         {
        //             if (ext == acceptedFileTypes[i])
        //             {
        //                 IsAccceptExt = true;
        //             }
        //         }
        //         if (!IsAccceptExt)
        //         {
        //             StatusMessage = "The file you are trying to upload is not a permitted file type!";
        //             return Redirect(Request.Headers["Referer"].ToString());
        //         }
        //         var root = env.ContentRootPath;
        //         var fileName = userId + "_" + Guid.NewGuid().ToString("N") + ext;
        //         // full path to file in temp location
        //         var filePath = Path.Combine(root, "Uploads", "file-CV", fileName); //we are using Temp file name just for the example. Add your own file path.
        //         using (var stream = new FileStream(filePath, FileMode.Create))
        //         {
        //             file.CopyTo(stream);


        //             _appDbContext.applyPosts.Add(new ApplyPost()
        //             {
        //                 OriginFileName = file.FileName,
        //                 ApplyDate = DateTime.Now,
        //                 Description = Description,
        //                 FileName = fileName,
        //                 UserID = userId,
        //                 Name = Name,
        //                 PostID = post.PostId,

        //             });
        //             _appDbContext.SaveChanges();
        //         }
        //     }


        //     StatusMessage = "Tải CV Thành Công";
        //     // return Redirect(Request.Headers["Referer"].ToString());
        //     return RedirectToAction("Index", "Home");
        // }
        // public IActionResult Search(string key, string city, string[] level, string[] type, string salary, string[] workspace)
        // {
        //     // ViewData["levels"] = _appDbContext.Levels.ToList();

        //     ViewData["key"] = key;
        //     ViewData["city"] = city;
        //     ViewData["level"] = level;
        //     ViewData["salary"] = salary;
        //     ViewData["type"] = type;
        //     ViewData["workspace"] = workspace;
        //     //ViewData["level"] = level;

        //     var listProvices = _appDbContext.Provinces.ToList();
        //     var topProvinces = _appDbContext.Provinces.Where(p => p.Code == "01" || p.Code == "48" || p.Code == "79").ToList();
        //     listProvices.RemoveAll(p => topProvinces.Contains(p));
        //     listProvices.InsertRange(0, topProvinces);


        //     var listProviceSelects = new SelectList(listProvices, "Code", "FullName", city);
        //     ViewData["listLevel"] = _appDbContext.Levels.ToList();
        //     ViewData["listProvices"] = listProvices;
        //     ViewData["provices"] = listProviceSelects;
        //     ViewData["posts"] = _appDbContext.Posts
        //         .Include(p => p.Company)
        //         .Include(p => p.PostSkills)
        //         .OrderBy(c => c.PostId)
        //         .Take(8)
        //         .AsSplitQuery()
        //         .ToList();
        //     var result = _appDbContext.Posts
        //         .Include(p => p.Address)
        //           .Include(p => p.Company)
        //          .Include(p => p.PostSkills)
        //          .ThenInclude(p => p.Skill)
        //          .Include(p => p.PostLevels)
        //          .ThenInclude(p => p.Level)
        //          .OrderBy(c => c.PostId)
        //          .AsSplitQuery();

        //     var b = result.ToList();
        //     if (!string.IsNullOrEmpty(key))
        //     {
        //         result = result.Where(p => p.Title.Contains(key));
        //     }
        //     if (workspace.Count() != 0)
        //     {
        //         result = result.Where(p => workspace.Contains(p.WorkSpace));
        //     }
        //     if (type.Count() != 0)
        //     {
        //         result = result.Where(p => type.Contains(p.Company.Type));
        //     }
        //     if (!string.IsNullOrEmpty(city))
        //     {

        //         if (city != "-1")
        //         {
        //             // result = result.Where(p => p.Company.Addresses.Select(a => a.City.Code).Contains(city));
        //             result = result.Where(p => p.Address.City.Code == city);

        //         }
        //     }
        //     if (!string.IsNullOrEmpty(salary))
        //     {
        //         if (salary != "-1")
        //         {

        //             var salaryLevel = int.Parse(salary);
        //             if (salaryLevel == 1)
        //             {
        //                 result = result.Where(p => p.MaxSalary <= 5000000);
        //             }
        //             else if (salaryLevel == 2)
        //             {
        //                 result = result.Where(p => p.MinSalary > 5000000 && p.MaxSalary <= 15000000);
        //             }
        //             else if (salaryLevel == 3)
        //             {
        //                 result = result.Where(p => p.MinSalary > 15000000 && p.MaxSalary <= 35000000);
        //             }
        //             else if (salaryLevel == 4)
        //             {
        //                 result = result.Where(p => p.MinSalary > 35000000);
        //             }

        //         }

        //     }
        //     if (level.Count() != 0)
        //     {
        //         /*

        //              var lq = from res in result
        //                  join postlevel in _appDbContext.PostLevels
        //                  on res.PostId equals postlevel.PostID
        //                  where level.Contains(postlevel.LevelID.ToString())
        //                  select res;
        //          */
        //         var lq1 = result
        //          .Join(
        //              _appDbContext.PostLevels,
        //              res => res.PostId,
        //              postlevel => postlevel.PostID,
        //              (res, postlevel) => new { res, postlevel }
        //          )
        //          .Where(joinResult => level.Contains(joinResult.postlevel.LevelID.ToString()))
        //          .Select(joinResult => joinResult.res);

        //         result = lq1;

        //     }

        //     ViewData["result"] = result.ToList();

        //     return View();
        // }
        // public IActionResult Condition()
        // {
        //     return View();
        // }

        // [Route("/contact-us")]
        // public IActionResult ContactUs()
        // {
        //     return View();
        // }
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        // }


        // public IActionResult ChangeLanguage(string language)
        // {
        //     if (!string.IsNullOrEmpty(language))
        //     {
        //         Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
        //         Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
        //     }
        //     else
        //     {
        //         language = "en-US";
        //         Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
        //         Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
        //     }
        //     Response.Cookies.Append("Language", language);
        //     return Redirect(Request.GetTypedHeaders().Referer.ToString());
        // }
    }
}