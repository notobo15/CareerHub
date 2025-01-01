using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApp.Models
{
    public class Language
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
