using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class Language
    {
        [Key]
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
