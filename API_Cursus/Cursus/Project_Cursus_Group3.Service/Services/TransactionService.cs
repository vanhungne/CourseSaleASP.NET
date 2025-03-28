using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
    public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<bool> DeleteTransaction(int transactionId)
        {
            var transaction = await _transactionRepository.DeleteTransactionAsync(transactionId);
            return transaction != null;
        }

    }
}
