using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Chapter
    {
        [Key]
        public int ChapterId { get; set; }
        public int? CourseId { get; set; }
        public string? ChapterTitle { get; set; }
        public string? SubDescription { get; set; }
        public string? Description { get; set; }
        public double? Process { get; set; }
        public int? Duration { get; set; }
        public string? Status { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public virtual ICollection<Lesson>? Lessons { get; set; }
    }
}
