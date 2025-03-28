using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public int? Star { get; set; }
        public string? Attachment { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public int? CourseId { get; set; }
        public string? UserName { get; set; }

        // Navigation property
        [ForeignKey("CourseId, UserName")]
        public PurchasedCourse PurchasedCourse { get; set; }

    }
}
