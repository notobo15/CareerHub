using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Location
    {
        public int LocationId { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }


        public int  CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
