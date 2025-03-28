using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly CursusDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public OrderRepository(CursusDbContext dbContext ,IUnitOfWork unitOfWork) : base(dbContext)
        {
            _unitOfWork = unitOfWork;
            _context = dbContext;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if(order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            try
            {
                _unitOfWork.BeginTransaction();
                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                _unitOfWork.CommitTransaction();
                return order;
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                throw new Exception("An error occurred while creating the order.", ex);
            }
        }
    }
}
