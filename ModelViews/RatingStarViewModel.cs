namespace RecruitmentApp.ModelViews
{
   public class RatingStarViewModel
{
    public int Rating { get; set; } = 0;         // Số sao được tô
    public int MaxStars { get; set; } = 5;       // Số sao tối đa
    public string ContainerClass { get; set; }   // Class cho <div>
    public string IconClass { get; set; }        // Class cho <i>
}
}
