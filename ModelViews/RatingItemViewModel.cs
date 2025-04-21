namespace RecruitmentApp.ModelViews
{
    public class RatingItemViewModel
    {
        public string Label { get; set; }
        public int Rating { get; set; }

        public RatingItemViewModel(string label, int rating)
        {
            Label = label;
            Rating = rating;
        }
    }
}
