using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }
        public int? ChapterId { get; set; }
        public string? VideoURL { get; set; }
        public int? Duration { get; set; }
        public double? Process { get; set; }
        public string? Description { get; set; }
        public string? LessonTitle { get; set; }
        public string Status { get; set; }

        [ForeignKey("ChapterId")]
        public Chapter? Chapter { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}
