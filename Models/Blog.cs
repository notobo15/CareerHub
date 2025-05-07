using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace RecruitmentApp.Models
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống.")]
        public string Content { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? Slug { get; set; }

        [StringLength(255)]
        public string? ThumbnailUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [Range(1, 100, ErrorMessage = "Thời gian đọc phải từ 1 đến 100 phút.")]
        public int ReadTimeMinutes { get; set; }

        public bool IsPublished { get; set; } = false;

        // Optional: Gắn user viết blog
        public string? AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public AppUser? Author { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public BlogCategory? Category { get; set; }

        public ICollection<BlogView> BlogViews { get; set; }
        public bool IsShowOnHome { get; set; } = false;
    }
}
