using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Models;
using RecruitmentApp.Models.Email;
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

            modelBuilder.Entity<CompanySkills>(entity =>
            {
                entity.HasKey(c => new { c.SkillID, c.CompanyID });
            });


            modelBuilder.Entity<PostSkills>(entity =>
            {
                entity.HasKey(c => new { c.SkillID, c.PostID });
            });
            modelBuilder.Entity<PostLevel>(entity =>
            {
                entity.HasKey(c => new { c.LevelID, c.PostID });
            });

            modelBuilder.Entity<Follower>(entity =>
            {
                entity.HasKey(c => new { c.UserId, c.CompanyId });
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
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


            // Thiết lập quan hệ Company - Post (1:N)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Company)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa công ty sẽ xóa bài đăng

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Location)
                .WithMany() // Một Location có thể có nhiều Post
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostView>()
                .HasIndex(pv => new { pv.PostId, pv.UserId, pv.IpAddress }) // Tạo index tránh trùng lặp
                .IsUnique();

            modelBuilder.Entity<Location>()
                .HasKey(l => l.LocationId);

            // Thiết lập quan hệ với Address (One-to-One)
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Address)
                .WithOne()
                .HasForeignKey<Location>(l => l.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ với Company (Many-to-One)
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Company)
                .WithMany(c => c.Locations)
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Address>()
                .HasOne(a => a.City)
                .WithMany()
                .HasForeignKey(a => a.ProvinceCode)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.District)
                .WithMany()
                .HasForeignKey(a => a.DistrictCode)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.Ward)
                .WithMany()
                .HasForeignKey(a => a.WardCode)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(e => e.User)
                .WithMany() // You can specify .WithMany(u => u.Reviews) if AppUser has ICollection<Review>
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Mail>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Subject).IsRequired();
                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.SentMails)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MailRecipient
            modelBuilder.Entity<MailRecipient>(entity =>
            {
                entity.HasKey(mr => new { mr.MailId, mr.UserId });
                entity.HasOne(mr => mr.Mail)
                    .WithMany(m => m.Recipients)
                    .HasForeignKey(mr => mr.MailId);
                entity.HasOne(mr => mr.User)
                    .WithMany(u => u.ReceivedMails)
                    .HasForeignKey(mr => mr.UserId);
            });

            // Attachment
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.FileName).IsRequired();
                entity.Property(a => a.FilePath).IsRequired();
                entity.HasOne(a => a.Mail)
                    .WithMany(m => m.Attachments)
                    .HasForeignKey(a => a.MailId);
            });

            // Label
            modelBuilder.Entity<Label>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Name).IsRequired();
                entity.HasOne(l => l.User)
                    .WithMany(u => u.Labels)
                    .HasForeignKey(l => l.UserId);
            });

            // MailLabel
            modelBuilder.Entity<MailLabel>(entity =>
            {
                entity.HasKey(ml => new { ml.MailId, ml.LabelId });
                entity.HasOne(ml => ml.Mail)
                    .WithMany(m => m.MailLabels)
                    .HasForeignKey(ml => ml.MailId);
                entity.HasOne(ml => ml.Label)
                    .WithMany(l => l.MailLabels)
                    .HasForeignKey(ml => ml.LabelId);
            });
        }

        /// <summary>
        ///  Email
        /// </summary>
        public DbSet<Mail> Mails { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<MailLabel> MailLabels { get; set; }
        public DbSet<MailRecipient> MailRecipients { get; set; }


        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CompanySkills> CompanySkills { get; set; }

        public DbSet<PostSkills> PostSkills { get; set; }
        public DbSet<PostLevel> PostLevels { get; set; }
        public DbSet<PostLocations> PostLocations { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ApplyPost> ApplyPosts { get; set; }
        public DbSet<Industry> Industries { get; set; }

        public DbSet<CompanyIndustry> CompanyIndustries { get; set; }

        public DbSet<PostView> PostViews { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<PostWorkType> PostWorkTypes { get; set; }
        public DbSet<ResumeFile> ResumeFiles { get; set; }

        public DbSet<PreferredLocation> PreferredLocations { get; set; }
        public DbSet<UserIndustry> UserIndustries { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<PersonalProject> PersonalProjects { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }
        public DbSet<UserAppliedPost> UserAppliedPosts { get; set; }
        public DbSet<UserSavedPost> UserSavedPosts { get; set; }
        public DbSet<UserViewedPost> UserViewedPosts { get; set; }
        public DbSet<BlockCompanyInvitation> BlockCompanyInvitations { get; set; }
        public DbSet<PostTitle> PostTitles { get; set; }
        public DbSet<PostIndustry> PostIndustries { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
    }
}