using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.ModelViews
{
    public class CoverLetterViewModel
    {
        [MaxLength(500, ErrorMessage = "Tối đa 500 ký tự.")]
        public string CoverLetter { get; set; }
    }
}
