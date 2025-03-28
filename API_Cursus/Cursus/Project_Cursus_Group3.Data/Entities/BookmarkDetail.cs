using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class BookmarkDetail
    {
        [Key, Column(Order = 0)]
        public int? BookmarkId { get; set; }
        [Key, Column(Order = 1)]
        public int? CourseId { get; set; }
        public string? Status { get; set; }
        [ForeignKey("BookmarkId")]
        public Bookmark? Bookmark { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }


    }
}
