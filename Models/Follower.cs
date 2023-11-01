using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Follower
    {
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public AppUser User { get; set; }
        public int CompanyID { get; set; }
       [ForeignKey("CompanyID")]
        public Company Company { get; set; }

    }
}
