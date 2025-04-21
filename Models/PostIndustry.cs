using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class PostIndustry
    {
        [Key]
        public int Id { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public int IndustryId { get; set; }
        public Industry Industry { get; set; }
    }
}
