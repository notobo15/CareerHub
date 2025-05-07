namespace RecruitmentApp.Utilities
{
    public static class ViewHelpers
    {
        public static string GetGenderText(int gender)
        {
            return gender switch
            {
                0 => "Nữ",
                1 => "Nam",
                _ => "Khác"
            };
        }
    }
}
