using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.DTOs
{
    public class WorkTypeDTO
    {
        public int WorkTypeId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
