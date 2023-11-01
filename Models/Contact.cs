using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Phai Nhap {0}")]
        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Display(Name = "Ho Ten")]
        public string FullName { get; set; }

        [StringLength(50)]
        [Phone(ErrorMessage = "Phai La {0}")]
        [Required]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Phai La {0}")]
        public string Email { get; set; }
        public DateTime? DateSend { get; set; }


        public string Message { get; set; }
    }
}
