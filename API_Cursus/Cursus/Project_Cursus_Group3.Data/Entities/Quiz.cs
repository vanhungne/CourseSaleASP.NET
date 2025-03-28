using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }
        public int LessonId { get; set; }
        public string? QuizTitle { get; set; }
        public string? Description { get; set; }
        public int? QuizTime { get; set; }
        public int? PassScore { get; set; }

        [ForeignKey("LessonId")]
        public Lesson Lesson { get; set; }
        public virtual ICollection<QuizAttemp> QuizAttemps { get; set; }
        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
}
