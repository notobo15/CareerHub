using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using RecruitmentApp.Data;
using RecruitmentApp.Models;
using RecruitmentApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
// dotnet aspnet-codegenerator controller -name PostController -m RecruitmentApp.Models.Company -dc AppDbContext -outDir Controllers -namespace RecruitmentApp.Controllers
namespace RecruitmentApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
			  services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender, SendMailService>();


            services.AddControllersWithViews();
            services.AddRazorPages();
            // Đăng ký AppDbContext, sử dụng kết nối đến MS SQL Server
            services.AddDbContext<AppDbContext>(options => {
               string connectstring = Configuration.GetConnectionString("DbContext");
                options.UseSqlServer(connectstring);
                //options.UseMySql(connectstring, ServerVersion.AutoDetect(connectstring));
            });
            // Đăng ký các dịch vụ của Identity
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true; // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại

            });

            // Cấu hình Cookie
            services.ConfigureApplicationCookie(options => {
                // options.Cookie.HttpOnly = true;  
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.LoginPath = $"/login/";                                 // Url đến trang đăng nhập
                options.LogoutPath = $"/logout/";
                options.AccessDeniedPath = $"/Account/AccessDenied";   // Trang khi User bị cấm truy cập
            });
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // Trên 5 giây truy cập lại sẽ nạp lại thông tin User (Role)
                // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
                options.ValidationInterval = TimeSpan.FromSeconds(5);
            });

            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

            services.AddAuthorization(options =>
            {
                // User thỏa mãn policy khi có roleclaim: permission với giá trị manage.user
                options.AddPolicy("AdminDropdown", policy => {
                    options.AddPolicy("ViewManageMenu", builder => {
                        builder.RequireAuthenticatedUser();
                        builder.RequireRole(RoleName.Administrator);
                    });
                    policy.RequireClaim("permission", "manage.user");
                });

            });

            services.AddAuthentication()
                 .AddGoogle(options => {
                     var gconfig = Configuration.GetSection("Authentication:Google");
                     options.ClientId = gconfig["ClientId"];
                     options.ClientSecret = gconfig["ClientSecret"];
                     // https://localhost:5001/signin-google
                     options.CallbackPath = "/dang-nhap-tu-google";
                 })
                 .AddFacebook(options => {
                     var fconfig = Configuration.GetSection("Authentication:Facebook");
                     options.AppId = fconfig["AppId"];
                     options.AppSecret = fconfig["AppSecret"];
                     options.CallbackPath = "/dang-nhap-tu-facebook";
                 })
                 // .AddTwitter()
                 // .AddMicrosoftAccount()
                 ;

            // Lowercase routes
            // services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddScoped<ExcelService>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

			app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                  Path.Combine(Directory.GetCurrentDirectory(), "Uploads")
              ),
                RequestPath = "/contents"
            });

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/{controller=Home}/{action=Index}/{id?}");
              
              //  endpoints.MapControllerRoute(
               //    name: "Recruiters",
               //    pattern: "nha-tuyen-dung/{controller=ManageInfo}/{action=Index}/{id?}",
               //    defaults: new { area = "Recruiters" });

               // endpoints.MapAreaControllerRoute(
              //    name: "Admin",
              //    areaName: "Admin",
              //    pattern: "admin/{controller=Home}/{action=Index}/{id?}");

            });

        }
    }
}
/*
 
 dotnet aspnet-codegenerator controller -name LevelController -m RecruitmentApp.Models.Level -dc AppDbContext -outDir Areas/Levels/Controllers -namespace RecruitmentApp.Areas.Levels.Controllers -l _AdminLayout
 dotnet aspnet-codegenerator controller -name PostController -m RecruitmentApp.Models.Post -dc AppDbContext -outDir Areas/Posts/Controllers -namespace RecruitmentApp.Areas.Posts.Controllers 
 dotnet aspnet-codegenerator controller -name SkillController -m RecruitmentApp.Models.Skill -dc AppDbContext -outDir Areas/Skills/Controllers -namespace RecruitmentApp.Areas.Skills.Controllers
 

dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.13
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.13
dotnet add package Microsoft.EntityFrameworkCore.SqlServer.Design --version 1.1.6
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.13
 */