using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class PostLevel
    {

        public int PostID { get; set; }

        public int LevelID { get; set; }

        [ForeignKey("LevelID")]
        public Level Level { get; set; }

        [ForeignKey("PostID")]
        public Post Post { get; set; }

 
    }
}
