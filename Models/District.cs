﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class District
    {
        [Key]
        [Column("code")]
        public string Code { get; set; }
        [Column("name")]
        [DisplayName("Tên Quận/Huyện")]
        public string Name { get; set; }
        [Column("full_name")]
        public string FullName { get; set; }

        [Column("province_code")]
        public string ProvinceId { get; set; }
        public Province Province { get; set; }

        [Column("name_en")]
        public string NameEn { get; set; }

        [Column("full_name_en")]
        public string FullNameEn { get; set; }

        [Column("code_name")]
        public string CodeName { get; set; }

        [Column("administrative_unit_id")]
        public int administrative_unit_id { get; set; }

        [Column("administrative_region_id")]
        public string administrative_region_id { get; set; }

        public List<Ward> Wards { get; set; }
    }
}
