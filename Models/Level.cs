using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class Level
    {
        [Key]
        public int LevelId { get; set; }
        [Required]
        [Display(Name = "Tên level")]
        public string Name { get; set; }

        public ICollection<PostLevel> PostLevels { get; set; }
    }
}
