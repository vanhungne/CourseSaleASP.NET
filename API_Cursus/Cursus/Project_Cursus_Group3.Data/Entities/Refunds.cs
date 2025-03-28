using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Refunds
    {
        [Key]
        public int RefundId { get; set; }

        public int OrderId { get; set; }

        public string TransactionId { get; set; }

        public decimal Amount { get; set; }

        public DateTime RefundDate { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
