using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IReasonServices
    {
        Task<Reason> GetByIdAsync(int id);
        Task<Reason> AddAsync(Reason reason);
        Task<Reason> UpdateAsync(int id, UpdateReasonModel reason);

        Task<Reason> DeleteAsync(int reportId);
        Task<List<Reason>> GetReasonsByUsernameAsync(string username, string reasonContent = null, string courseTitle = null);
    }
}
