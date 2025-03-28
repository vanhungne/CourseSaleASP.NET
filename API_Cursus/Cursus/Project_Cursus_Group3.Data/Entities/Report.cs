using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        public string? Issue { get; set; }
        public string? Attachment { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }

        public int? CourseId { get; set; }
        public string? UserName { get; set; }

        // Navigation property
        [ForeignKey("CourseId, UserName")]
        public PurchasedCourse PurchasedCourse { get; set; }
    }
}
