using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models.RecruitmentApp.Models;
using System.Linq;
namespace RecruitmentApp.Models 
{
    // App.Models.AppDbContext
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
          
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

             foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            /*
             *   modelBuilder.Entity<Post>()
               .HasMany<Skill>(s => s.Skills)
               .WithMany(c => c.Posts);
             * 
             *  modelBuilder.Entity<Post>()
              .HasMany<Title>(s => s.Titles)
              .WithMany(c => c.Posts);
            modelBuilder.Entity<Post>()
             .HasMany<Level>(s => s.Levels)
             .WithMany(c => c.Posts);
             * 
             * 
             * 
               modelBuilder.Entity<Company>()
            .HasMany<Skill>(s => s.Skills)
            .WithMany(c => c.Companies);

                 modelBuilder.Entity<AppUser>()
           .HasMany<Company>(s => s.Followers)
           .WithMany(c => c.Followers);
             
             */

            modelBuilder.Entity<CompanySkills>(entity => {
                entity.HasKey(c => new { c.SkillID, c.CompanyID });
            });


            modelBuilder.Entity<PostSkills>(entity => {
                entity.HasKey(c => new { c.SkillID, c.PostID });
            });
            modelBuilder.Entity<PostLevel>(entity => {
                entity.HasKey(c => new { c.LevelID, c.PostID });
            });

            modelBuilder.Entity<Follower>(entity => {
                entity.HasKey(c => new { c.UserID, c.CompanyID });
            });

            modelBuilder.Entity<Favorite>(entity => {
                entity.HasKey(c => new { c.UserID, c.PostID });
            });


            /*
            modelBuilder.Entity<ApplyPost>(entity => {
                entity.HasKey(c => new { c.UserID, c.PostID, c.FileName });
            });



            

           modelBuilder.Entity<ApplyPost>(entity => {
                entity.HasKey(c => new { c.UserID, c.PostID, c.FileName });
            });
            modelBuilder.Entity<District>()
                .Property(x => x.Province).HasColumnName("province_code");

            modelBuilder.Entity<Ward>()
               .Property(x => x.District).HasColumnName("district_code");
             * 
             * 
             * 
             * 
             * 




             *      
           modelBuilder.Entity<PostSkills>(entity => {
                entity.HasKey(c => new { c.SkillID, c.PostID });
            });

             * 
             * 

                modelBuilder.Entity<Follower>(entity =>
            {
                entity.HasIndex(ee => new { ee.UserId, ee.CompanyId})
                .IsUnique();
            });
              modelBuilder.Entity<AppUser>()
                .HasMany<Post>(e => e.Favourates)
                .WithMany(e => e.Favourites);

             */

            modelBuilder.Entity<CompanyIndustry>()
           .HasKey(ci => new { ci.CompanyId, ci.IndustryId });

            modelBuilder.Entity<CompanyIndustry>()
                .HasOne(ci => ci.Company)
                .WithMany(c => c.CompanyIndustries)
                .HasForeignKey(ci => ci.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyIndustry>()
                .HasOne(ci => ci.Industry)
                .WithMany(i => i.CompanyIndustries)
                .HasForeignKey(ci => ci.IndustryId)
                .OnDelete(DeleteBehavior.Cascade);


            // Thiết lập quan hệ Company - Address (1:N)
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Company)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa công ty sẽ xóa địa chỉ

            // Thiết lập quan hệ Address - Post (1:N)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Address)
                .WithMany(a => a.Posts)
                .HasForeignKey(p => p.AddressId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa địa chỉ nếu có bài đăng sử dụng nó

            // Thiết lập quan hệ Company - Post (1:N)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Company)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa công ty sẽ xóa bài đăng


            modelBuilder.Entity<PostView>()
              .HasIndex(pv => new { pv.PostId, pv.UserId, pv.IpAddress }) // Tạo index tránh trùng lặp
              .IsUnique();
        }
        // db provices

        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }


        public DbSet<Contact> Contact { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CompanySkills> CompanySkills { get; set; }

        public DbSet<PostSkills> PostSkills { get; set; }
        public DbSet<PostLevel> PostLevels { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ApplyPost> applyPosts { get; set; }
        public DbSet<Industry> Industries { get; set; }

        public DbSet<CompanyIndustry> CompanyIndustries { get; set; }

        public DbSet<PostView> PostViews { get; set; }
    }
}