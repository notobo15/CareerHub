using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Level
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LevelId { get; set; }
        [Required]
        [Display(Name = "Tên level")]
        public string Name { get; set; }

        public ICollection<PostLevel> PostLevels { get; set; }
    }
}
