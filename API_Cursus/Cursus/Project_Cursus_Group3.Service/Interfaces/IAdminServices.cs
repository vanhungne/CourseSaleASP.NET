using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IAdminServices
    {
        Task<User?> GetByIdAsync(string userName);
        Task UpdateAsync(User user);
        Task ToggleUserStatusAsync(string userName, bool isActive, string comment);
        Task ConfirmReasonAsync(int courseId, bool isApproved);
    }
}
