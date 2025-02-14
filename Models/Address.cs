using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int AddressId { get; set; }
        public string Nation { get; set; }

        // [ForeignKey("Code")]

      //  [Column("ProvinceCODE")]
       public string ProvinceCode { get; set; }
        public Province City { get; set; }

        // [ForeignKey("Code")]
      //  [Column("DistrictCODE")]
        public string DistrictCode { get; set; }
        public District District { get; set; }

        public string WardCode { get; set; }
        public Ward Ward { get; set; }
        public string DetailPosition { get; set; }
       public string GgMapSrc { get; set; }

        public string FullAddress { get; set; }

        [ForeignKey("CompanyId")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<Post> Posts { get; set; }

    }
}
