using RecruitmentApp.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RecruitmentApp.Services
{
    public class ReviewStatsService
    {
        public ReviewSummaryStats GetFullReviewStats(List<Review> reviews)
        {
            return new ReviewSummaryStats
            {
                Overall = CalculateStats(reviews.Select(r => r.OverallRating).ToList()),
                Salary = CalculateStats(reviews.Select(r => r.SalaryRating).ToList()),
                Training = CalculateStats(reviews.Select(r => r.TrainingRating).ToList()),
                Management = CalculateStats(reviews.Select(r => r.ManagementRating).ToList()),
                Culture = CalculateStats(reviews.Select(r => r.CultureRating).ToList()),
                Office = CalculateStats(reviews.Select(r => r.OfficeRating).ToList())
            };
        }

        private RatingDetailStats CalculateStats(List<int> ratings)
        {
            var result = new RatingDetailStats();
            if (ratings == null || ratings.Count == 0) return result;

            result.Average = Math.Round(ratings.Average(), 2);
            var total = ratings.Count;

            for (int i = 1; i <= 5; i++)
            {
                var count = ratings.Count(r => r == i);
                result.Percentages[i] = Math.Round((double)count / total * 100, 2);
            }

            return result;
        }
    }
}
