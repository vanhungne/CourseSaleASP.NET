using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class PurchasedCourse
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int PurchasedCourseId { get; set; }
        public int CourseId { get; set; }
        public string UserName { get; set; }

        public string Status { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        [ForeignKey("UserName")]
        public User User { get; set; }

        public ICollection<Feedback> Feedback { get; set; }
        public Report Report { get; set; }


    }
}
