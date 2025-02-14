using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Favorite
    {
        public string UserID { get; set; }
        public int PostID { get; set; }
        [ForeignKey("PostID")]
        public Post Post { get; set; }
        [ForeignKey("UserID")]
        public AppUser User { get; set; }
    }
}
