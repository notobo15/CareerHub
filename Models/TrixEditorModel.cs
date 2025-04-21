using System.Globalization;

namespace RecruitmentApp.Models
{
    public class TrixEditorModel
    {
        public string AspFor { get; set; } = string.Empty;

        public string? Value { get; set; }

        public int Height { get; set; } = 250;

        public int MaxLength { get; set; } = 2500;

        public string? Placeholder { get; set; }

        public bool EnableCounter { get; set; } = true;

        public bool Disabled { get; set; } = false;

        public string? ToolbarId { get; set; } // Optional override
    }
}
