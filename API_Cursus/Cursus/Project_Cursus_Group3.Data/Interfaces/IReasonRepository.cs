using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project_Cursus_Group3.Data.Model.ReasonModel;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface IReasonRepository
    {
        Task<Reason> GetByIdAsync(int id);

    
        Task<Reason?> DeleteAsync(int id);

        Task<Reason> AddAsync(Reason reason);
        Task<Reason> UpdateAsync(int id, UpdateReasonModel reason);
        Task<List<Reason>> GetReasonsByUsernameAsync(string username, string reasonContent = null, string courseTitle = null);
    }
}