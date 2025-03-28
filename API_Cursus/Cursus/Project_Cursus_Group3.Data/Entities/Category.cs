using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Status { get; set; }
        public virtual ICollection<Category>? SubCategories { get; set; }
        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { get; set; }

        public virtual ICollection<Course>? Courses { get; set; }
    }
}
