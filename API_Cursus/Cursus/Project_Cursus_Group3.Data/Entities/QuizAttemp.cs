using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class QuizAttemp
    {
        [Key]
        public int AttempId { get; set; }
        public string? UserName { get; set; }
        public int? QuizId { get; set; }
        public int? Score { get; set; }
        public int? CompletedTime { get; set; }

        [ForeignKey("UserName")]
        public User User { get; set; }
        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

    }
}
