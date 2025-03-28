using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels.TransactionsDTO
{
    public class TransactionsViewModel
    {
        public int TransactionId { get; set; }
        public int walletId { get; set; }
        public string? PaymentCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public double? Amount { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

    }
}
