using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RecruitmentApp.Models 
{
    public class AppUser: IdentityUser 
    {
          [Column(TypeName = "nvarchar")]
          [StringLength(400)]  
          public string HomeAdress { get; set; }

          // [Required]       
          [DataType(DataType.Date)]
          public DateTime? BirthDate { get; set; }

          [Column(TypeName = "TINYINT")]
          public int Gender { get; set; }

          public string AcademicLevel { get; set; }

          public string PersonLink { get; set; }  

          public virtual ICollection<Post> Posts { get; set; }
            //public virtual ICollection<Post> Favourates { get; set; }
          public ICollection<Follower> Followers { get; set; }

        public ICollection<Favorite> Favorites { get; set; }
    }
}
