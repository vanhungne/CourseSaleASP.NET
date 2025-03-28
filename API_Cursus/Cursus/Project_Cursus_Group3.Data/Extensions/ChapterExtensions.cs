using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Extensions
{
    public static class ChapterExtensions
    {
        public static IQueryable<Chapter> FilterByStatus(this IQueryable<Chapter> query, string status)
        {
            return query.Where(c => c.Status == status);
        }

        public static IQueryable<Chapter> SearchByTitle(this IQueryable<Chapter> query, string? searchContent)
        {
            if (string.IsNullOrEmpty(searchContent)) return query;

            var lowerCaseSearchTerm = searchContent.Trim().ToLower();
            return query.Where(c => c.ChapterTitle.ToLower().Contains(lowerCaseSearchTerm) ||
                                    c.Course.CourseTitle.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Chapter> FilterByDuration(this IQueryable<Chapter> query, int? duration)
        {
            if (duration == null) return query;

            return query.Where(c => c.Duration == duration);
        }

        public static IQueryable<Chapter> ApplyPaging(this IQueryable<Chapter> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
