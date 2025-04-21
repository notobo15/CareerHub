using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class ResumeFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResumeFileId { get; set; }


        [Required]
        public string FileName { get; set; }

        [Required]
        public string OrigialFileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPrimary { get; set; } = false;

        // Foreign key
        [Required]
        public string UserId { get; set; }

        // Navigation property
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }
    }
}
