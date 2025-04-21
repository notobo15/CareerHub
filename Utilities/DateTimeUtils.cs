using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace RecruitmentApp.Utilities
{
    public static class DateTimeUtils
    {
        public static string ToVietnameseDate(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        public static string? ToVietnameseDate(this DateTime? dateTime)
        {
            return dateTime?.ToString("dd/MM/yyyy");
        }
        public static string ToVietnameseDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss dd/MM/yyyy");
        }

        public static string? ToVietnameseDateTime(this DateTime? dateTime)
        {
            return dateTime?.ToString("HH:mm:ss dd/MM/yyyy");
        }
        public static List<SelectListItem> GetMonthList()
        {
            var list = Enumerable.Range(1, 12)
                .Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString("D2")
                })
                .ToList();

            list.Insert(0, new SelectListItem { Text = "", Value = "" });
            return list;
        }
        public static List<SelectListItem> GetYearList(int from = 1950, int extraFuture = 5)
        {
            int current = DateTime.Now.Year;

            var list = Enumerable.Range(from, current - from + extraFuture)
                .Reverse()
                .Select(y => new SelectListItem
                {
                    Value = y.ToString(),
                    Text = y.ToString()
                })
                .ToList();

            list.Insert(0, new SelectListItem { Text = "", Value = "" });
            return list;
        }
    }
}