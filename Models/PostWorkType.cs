using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class PostWorkType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostWorkTypeId { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }


        public int WorkTypeId { get; set; }
        public WorkType WorkType { get; set; }

    
    }
}
