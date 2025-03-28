using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Extensions
{
    public static class FeedbackExtensions
    {
        public static IQueryable<Feedback> FilterByStatus(this IQueryable<Feedback> query, string status)
        {
            return query.Where(f => f.Status == status);
        }

        public static IQueryable<Feedback> FilterByUserName(this IQueryable<Feedback> query, string userName)
        {
            return query.Where(f => f.UserName == userName);
        }

        public static IQueryable<Feedback> SearchContent(this IQueryable<Feedback> query, string? searchContent)
        {
            if (string.IsNullOrEmpty(searchContent)) return query;

            var lowerCaseSearchTerm = searchContent.Trim().ToLower();
            return query.Where(f => f.Content.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Feedback> Sort(this IQueryable<Feedback> query, string sortBy, bool ascending)
        {
            if (string.IsNullOrEmpty(sortBy)) return query;

            return sortBy.ToLower() switch
            {
                "createdate" => ascending ? query.OrderBy(f => f.CreatedDate) : query.OrderByDescending(f => f.CreatedDate),
                "star" => ascending ? query.OrderBy(f => f.Star) : query.OrderByDescending(f => f.Star),
                _ => query 
            };
        }

        public static IQueryable<Feedback> FilterByCourseId(this IQueryable<Feedback> query, int? courseId)
        {
            if (courseId == null) return query;

            return query.Where(f => f.CourseId == courseId);
        }
    }
}

