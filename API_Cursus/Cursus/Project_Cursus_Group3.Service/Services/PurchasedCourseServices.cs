using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Services
{
    public class PurchasedCourseServices : IPurchasedCourseServices
    {
        private readonly IPurchasedCourseRepository _purchasedCourseRepository;

        public PurchasedCourseServices(IPurchasedCourseRepository purchaseRepository)
        {
            _purchasedCourseRepository = purchaseRepository;
        }

        public async Task<List<int>> GetEnrolledCourseIdsByUserNameAsync(string userName)
        {
            return await _purchasedCourseRepository.GetEnrolledCourseIdsByUserNameAsync(userName);
        }
    }
}
