using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using RecruitmentApp.Areas.Admin.Email.Services;
using RecruitmentApp.Data;
using RecruitmentApp.Hubs;
using RecruitmentApp.Models;
using RecruitmentApp.Seed;
using RecruitmentApp.Services;
using RecruitmentApp.Services.Users;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


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

            services.AddLocalization(o => { o.ResourcesPath = "Resources"; });


            services.AddControllersWithViews()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("vi"),
                };

                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;



                // options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                // {
                //     // My custom request culture logic
                //     return await Task.FromResult(new ProviderCultureResult("en"));
                // }));
            });

            services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender, SendMailService>();


            services.AddRazorPages();

            services.AddDbContext<AppDbContext>(options =>
            {
                string connectstring = Configuration.GetConnectionString("DbContext");
                options.UseSqlServer(connectstring, sqlOptions => sqlOptions.EnableRetryOnFailure());
                //options.UseMySql(connectstring, ServerVersion.AutoDetect(connectstring));
            });

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
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
            services.ConfigureApplicationCookie(options =>
            {
            // options.Cookie.HttpOnly = true;  
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    var requestPath = context.Request.Path;

                    if (requestPath.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.Redirect("/admin/account/login?returnUrl=" + Uri.EscapeDataString(context.Request.Path));
                    }
                    else if (requestPath.StartsWithSegments("/", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.Redirect("/auth/login?returnUrl=" + Uri.EscapeDataString(context.Request.Path));
                    }
                    else
                    {
                        context.Response.Redirect("/login?returnUrl=" + Uri.EscapeDataString(context.Request.Path));
                    }

                    return Task.CompletedTask;
                },

                    OnRedirectToAccessDenied = context =>
                    {
                        // Cũng có thể xử lý tương tự cho trang AccessDenied nếu muốn
                        context.Response.Redirect("/access-denied");
                        return Task.CompletedTask;
                    }
                };



                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                //options.LoginPath = $"/admin/account/login";                                 // Url đến trang đăng nhập
                options.LogoutPath = $"/logout/";
                //options.AccessDeniedPath = $"/Account/AccessDenied";   // Trang khi User bị cấm truy cập
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
                options.AddPolicy("AdminDropdown", policy =>
                {
                    options.AddPolicy("ViewManageMenu", builder =>
                    {
                        builder.RequireAuthenticatedUser();
                        builder.RequireRole(RoleName.Admin);
                    });
                    policy.RequireClaim("permission", "manage.user");
                });

            });

            services.AddAuthentication()
                 .AddGoogle(options =>
                 {
                     var gconfig = Configuration.GetSection("Authentication:Google");
                     options.ClientId = gconfig["ClientId"];
                     options.ClientSecret = gconfig["ClientSecret"];
                     // https://localhost:5001/signin-google
                     options.CallbackPath = "/dang-nhap-tu-google";
                     options.ClaimActions.MapJsonKey("image", "picture");
                     //  options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
                     //  options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
                     //  options.Scope.Add("https://www.googleapis.com/auth/user.birthday.read");
                     //  options.Scope.Add("https://www.googleapis.com/auth/user.phonenumbers.read");

                     options.SaveTokens = true;
                 })
                 .AddFacebook(options =>
                 {
                     var fconfig = Configuration.GetSection("Authentication:Facebook");
                     options.AppId = fconfig["AppId"];
                     options.AppSecret = fconfig["AppSecret"];
                     options.CallbackPath = "/dang-nhap-tu-facebook";
                 })
                 ;
            using (var scope = services.BuildServiceProvider())
            {
                var dbContext = scope.GetRequiredService<AppDbContext>();
                var roleManager = scope.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.GetRequiredService<UserManager<AppUser>>();
                dbContext.Database.EnsureCreated();
                //dbContext.Database.Migrate();
                SeedDefault.SeedAsync(dbContext).Wait();
                IdentitySeed.SeedRolesAndAdminAsync(roleManager, userManager).Wait();
            }
            // Lowercase routes
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddScoped<HeaderService>();
            services.AddScoped<PostService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<PostService>();
            services.AddHttpClient(); // Để inject HttpClient
            services.AddScoped<ImageService>();
            services.AddSignalR();
            services.AddScoped<EmailStatsService>();
            services.AddScoped<UserService>();
            services.AddScoped<ReviewStatsService>();
            services.AddScoped<UploadFileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
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
                try
                {
                    using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                    serviceScope.ServiceProvider.GetService<AppDbContext>().Database.Migrate();
                }
                catch
                {
                }
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

            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            app.Use(async (context, next) =>
            {
                var cultureQuery = context.Request.Query["culture"];
                if (!string.IsNullOrWhiteSpace(cultureQuery))
                {
                    var culture = new CultureInfo(cultureQuery);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
                await next();
            }); 

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MailHub>("/mailhub");
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                    );
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

        }

    }

}
/*
dotnet aspnet-codegenerator controller -name PostController -m RecruitmentApp.Models.Company -dc AppDbContext -outDir Controllers -namespace RecruitmentApp.Controllers
 
 dotnet aspnet-codegenerator controller -name LevelController -m RecruitmentApp.Models.Level -dc AppDbContext -outDir Areas/Levels/Controllers -namespace RecruitmentApp.Areas.Levels.Controllers -l _AdminLayout
 dotnet aspnet-codegenerator controller -name PostController -m RecruitmentApp.Models.Post -dc AppDbContext -outDir Areas/Posts/Controllers -namespace RecruitmentApp.Areas.Posts.Controllers 
 dotnet aspnet-codegenerator controller -name SkillController -m RecruitmentApp.Models.Skill -dc AppDbContext -outDir Areas/Skills/Controllers -namespace RecruitmentApp.Areas.Skills.Controllers
 
 */