using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RecruitmentApp.Areas.Admin.Blogs.ViewModels
{
    public class BlogViewModel
    {
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
        [Display(Name = "Tải ảnh mới")]
        public IFormFile ThumbnailImage { get; set; }
        public int ReadTimeMinutes { get; set; }

        public bool IsPublished { get; set; }

        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        // Optional: nếu cần chọn tác giả từ dropdown
        public string? AuthorId { get; set; }

        // Gợi ý: dùng cho dropdown nếu cần
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? Categories { get; set; }


    }
}
