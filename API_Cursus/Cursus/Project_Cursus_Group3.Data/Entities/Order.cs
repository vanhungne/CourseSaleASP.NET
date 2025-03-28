using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string? UserName { get; set; }
        public string? OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public double? OrderPrice { get; set; }
        public string? Status { get; set; }

        [ForeignKey("UserName")]
        public User? User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
