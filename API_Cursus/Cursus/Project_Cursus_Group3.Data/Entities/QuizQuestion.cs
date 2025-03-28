using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class QuizQuestion
    {
        [Key]
        public int QuizQuestionId { get; set; }
        public int QuizId { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
        public virtual ICollection<QuizAnswer> QuizAnswers { get; set; }
    }
}
