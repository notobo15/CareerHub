using System.Collections.Generic;

namespace RecruitmentApp.Models
{
    public class RatingDetailStats
    {
        public double Average { get; set; }
        public Dictionary<int, double> Percentages { get; set; } = new();
    }
}
