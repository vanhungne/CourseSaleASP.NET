using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class OrderDetail
    {
        /*     [Key]
             [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
             public int OrderDetailId { get; set; }*/
        public int OrderId { get; set; }
        public int CourseId { get; set; }
        public double Price { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

    }
}
