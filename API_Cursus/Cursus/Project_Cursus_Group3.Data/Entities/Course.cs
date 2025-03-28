using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string? Username { get; set; }
        public int CategoryId { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
        public double Discount { get; set; }
        public int Level { get; set; }
        public bool IsComment { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? TotalEnrollment { get; set; }
        public string? Status { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }
        public string? ShortDescription { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [ForeignKey("Username")]
        public User User { get; set; }

        public virtual ICollection<PurchasedCourse> PurchasedCourses { get; set; }
        public virtual ICollection<BookmarkDetail> BookmarkDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Chapter> Chapters { get; set; }

        public virtual Reason Reason { get; set; }

    }
}
