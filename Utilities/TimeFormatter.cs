using System;

namespace RecruitmentApp.Utilities
{
    public static class TimeFormatter
    {
        public static string GetTimeAgo(DateTime pastTime)
        {
            TimeSpan timeDifference = DateTime.Now - pastTime;

            if (timeDifference.TotalMinutes < 60)
            {
                return $"{(int)timeDifference.TotalMinutes} phút trước";
            }
            else if (timeDifference.TotalHours < 24)
            {
                return $"{(int)timeDifference.TotalHours} giờ trước";
            }
            else
            {
                int days = (int)timeDifference.TotalDays;
                int hours = (int)(timeDifference.TotalHours % 24);
                return $"{days} ngày {hours} giờ trước";
            }

        }
    }
}
