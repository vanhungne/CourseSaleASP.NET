using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class TransactionRepository : Repository<Transactions>, ITransactionRepository
    {
        private readonly CursusDbContext _context;

        public TransactionRepository(CursusDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task AddPaymentAsync(Transactions transactions)
        {
           await _context.Transactions.AddAsync(transactions);
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            var transaction = await Entities.FindAsync(transactionId);
            if (transaction != null)
            {
                var reTransaction = new ReTransactions
                {
                    TransactionId = transaction.TransactionId,
                    WalletId = transaction.walletId,
                    PaymentCode = transaction.PaymentCode,
                    CreatedDate = transaction.CreatedDate,
                    Amount = transaction.Amount,
                    Description = transaction.Description,
                    Status = transaction.Status,
                    DeletedDate = DateTime.Now
                };

                _context.ReTransactions.Add(reTransaction);
                _context.Transactions.Remove(transaction);

                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}
