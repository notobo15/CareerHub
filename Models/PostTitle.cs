using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class PostTitle
    {
        [Key]
        public int Id { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public int TitleId { get; set; }
        public Title Title { get; set; }
    }
}
