using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly CursusDbContext _context;

        public PaymentRepository(CursusDbContext context) : base(context)
        {
            {
                _context = context;
            }
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payment.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public Task DeletePaymentAsync(int paymentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return  Entities.Include(p => p.Order).FirstOrDefault(P => P.PaymentId == paymentId);

        }

        public Task UpdatePaymentAsync(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}