using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddPaymentAsync(Transactions transactions);
        Task<bool> DeleteTransactionAsync(int transactionId);
    }
}
