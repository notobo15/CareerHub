using System.Globalization;

namespace RecruitmentApp.Models
{
    public class Summernote
    {
        public string Id { get; set; }
        public bool IsLoadLibrary { get; set; }
        public int Height { get; set; } = 350;


        public string Toolbar { get; set; } = @"[
                    ['style', ['style']],
                    ['font', ['bold', 'underline', 'clear']],
                    ['color', ['color']],
                    ['para', ['ul', 'ol', 'paragraph']],
                    ['table', ['table']],
                    ['insert', ['link', 'picture', 'video', 'elfinder']],
                    ['height', ['height']],
                    ['view', ['fullscreen', 'codeview', 'help']]
                ]";

        public Summernote(string id, bool IsLoadLibrary = true) {
            this.Id  = id;
            this.IsLoadLibrary = IsLoadLibrary;
        }
    }
}
