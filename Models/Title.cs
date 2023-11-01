using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class Title
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual List<Post> Posts { get; set; }
    }
}
