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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly CursusDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailRepository(CursusDbContext context, IUnitOfWork unitOfWork) : base(context) {
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }
        }
        public async Task CreateOrderDetailsAsync(IEnumerable<OrderDetail> orderDetails)
        {
            if (orderDetails == null)
            {
                throw new ArgumentNullException(nameof(orderDetails));
            }
            try
            {
                _unitOfWork.BeginTransaction();
                await _context.OrderDetail.AddRangeAsync(orderDetails);
                await _context.SaveChangesAsync();
                _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception("An error occurred while creating the order details.", ex);
            }
        }
    }
}
