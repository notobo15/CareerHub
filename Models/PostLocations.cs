using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class PostLocations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostLocationId { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public int PostID { get; set; }
        public Post Post { get; set; }


        public ICollection<PostLocations> Locations { get; set; }
    }
}
