using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class ApplyPost
    {
        [Key]
        public int Id { get; set; }

        public int PostID { get; set; }
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public AppUser User { get; set; }

        [ForeignKey("PostID")]
        public Post Post { get; set; }

        public string OriginFileName { get; set; }     // tên gốc
        public string FileName { get; set; }           // tên file đã lưu
        public string FilePath { get; set; }           // đường dẫn file

        public DateTime ApplyDate { get; set; } = DateTime.Now;

        [Required]
        public string Name { get; set; }               // Tên user
        [Required]
        public string Phone { get; set; }              // SĐT user
        public string Description { get; set; }        // Cover letter
    }
}
