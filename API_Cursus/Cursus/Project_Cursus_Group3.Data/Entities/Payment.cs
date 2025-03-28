using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string? PaymentCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public double? Amount { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
