using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "Phai Nhap {0}")]
        //[Column(TypeName = "nvarchar")]
        //[StringLength(50)]
        [Display(Name = "Họ Và Tên")]
        public string FullName { get; set; }

        [StringLength(50)]
        [Phone(ErrorMessage = "Phai La {0}")]
        [Required]
        [Display(Name = "Số Điện Thoại")]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Phai La {0}")]
        public string Email { get; set; }
        public DateTime? DateSend { get; set; }

        [Display(Name = "Lời nhắn")]
        public string Message { get; set; }
    }
}
