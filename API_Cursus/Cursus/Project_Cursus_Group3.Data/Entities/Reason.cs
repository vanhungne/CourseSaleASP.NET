using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Reason
    {
        [Key]
        public int ReasonId { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
        public int? CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

    }
}
