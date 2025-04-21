namespace RecruitmentApp.Models
{
    public class ReviewSummaryStats
    {
        public RatingDetailStats Overall { get; set; }
        public RatingDetailStats Salary { get; set; }
        public RatingDetailStats Training { get; set; }
        public RatingDetailStats Management { get; set; }
        public RatingDetailStats Culture { get; set; }
        public RatingDetailStats Office { get; set; }
    }
}
