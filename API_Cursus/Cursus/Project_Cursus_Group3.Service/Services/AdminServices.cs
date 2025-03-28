using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Repository
{
    public class AdminServices : IAdminServices
    {
        private readonly IAdminRepository _adminRepository;

        public AdminServices(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public Task<User?> GetByIdAsync(string userName)
        {
            return _adminRepository.GetByIdAsync(userName);
        }

        public Task UpdateAsync(User user)
        {
            return _adminRepository.UpdateAsync(user);
        }

        public Task ToggleUserStatusAsync(string userName, bool isActive, string comment)
        {
            return _adminRepository.ToggleUserStatusAsync(userName, isActive, comment);
        }

        public Task ConfirmReasonAsync(int courseId, bool isApproved)
        {
            return _adminRepository.ConfirmReasonAsync(courseId,isApproved);
        }
    }
}
