﻿namespace RecruitmentApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }
        public AppUser User { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tổng điểm.")]
        [Range(1, 5, ErrorMessage = "Overall rating must be between 1 and 5.")]
        public int OverallRating { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tóm tắt.")]
        [StringLength(500, ErrorMessage = "Summary must be at most 500 characters.")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Please select your satisfaction level.")]
        public bool IsOvertimeSatisfied { get; set; }

        [StringLength(140, MinimumLength = 50, ErrorMessage = "Reason must be between 50 and 140 characters.")]
        public string OvertimeReason { get; set; }

        [Required]
        [StringLength(10000, MinimumLength = 50, ErrorMessage = "Experience must be between 50 and 10000 characters.")]
        public string Experience { get; set; }

        [Required]
        [StringLength(10000, MinimumLength = 50, ErrorMessage = "Suggestion must be between 50 and 10000 characters.")]
        public string Suggestion { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Salary rating must be between 1 and 5.")]
        public int SalaryRating { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Training rating must be between 1 and 5.")]
        public int TrainingRating { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Management rating must be between 1 and 5.")]
        public int ManagementRating { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Culture rating must be between 1 and 5.")]
        public int CultureRating { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Office rating must be between 1 and 5.")]
        public int OfficeRating { get; set; }
        public string? Sentiment { get; set; }

        [Required]
        public bool RecommendToFriends { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(5000, ErrorMessage = "Reply must be at most 5000 characters.")]
        public string? CompanyReply { get; set; }
        public DateTime? RepliedAt { get; set; }
        public string? RepliedById { get; set; }

        public AppUser RepliedBy { get; set; }


        public string? SentimentModelName { get; set; }
    }
}
