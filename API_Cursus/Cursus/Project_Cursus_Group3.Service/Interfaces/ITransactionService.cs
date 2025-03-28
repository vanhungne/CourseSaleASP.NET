using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface ITransactionService
    {
        Task<bool> DeleteTransaction(int transactionId);
    }
}
