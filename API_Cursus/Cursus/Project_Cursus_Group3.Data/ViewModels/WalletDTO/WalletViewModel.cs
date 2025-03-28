using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project_Cursus_Group3.Data.ViewModels.TransactionsDTO;

namespace Project_Cursus_Group3.Data.ViewModels.WalletDTO
{
    public class WalletViewModel
    {
        public int WalletId { get; set; }
        public double? Balance { get; set; }
        public DateTime TransactionTime { get; set; }
        public string? UserName { get; set; }
        public List<TransactionsViewModel> Transactions { get; set; }
    }
}
