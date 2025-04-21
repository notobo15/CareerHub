using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class WorkType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkTypeId { get; set; }
        [Required]
        [Display(Name = "Tên hình thức làm việc")]
        public string Name { get; set; }
        public string Slug { get; set; }

        public ICollection<PostWorkType> PostWorkTypes { get; set; }
    }
}
