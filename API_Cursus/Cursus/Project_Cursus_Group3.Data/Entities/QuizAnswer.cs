using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class QuizAnswer
    {
        [Key]
        public int QuizAnswerId { get; set; }
        public int? QuizQuestionId { get; set; }
        public string? AnswerText { get; set; }
        public bool? IsCorrect { get; set; }
        [ForeignKey("QuizQuestionId")]
        public QuizQuestion QuizQuestion { get; set; }
    }
}
