using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.DTOs
{
    public class AddressDTO
    {
        public int AddressId { get; set; }
        public string Nation { get; set; }
       public string ProvinceCode { get; set; }
        public string Province { get; set; }

        public string DistrictCode { get; set; }
        public string District { get; set; }

        public string WardCode { get; set; }
        public string Ward { get; set; }
        public string DetailPosition { get; set; }
       public string GgMapSrc { get; set; }

        public string FullAddress { get; set; }

    }
}
