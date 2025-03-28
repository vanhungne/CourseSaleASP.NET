using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service
{
    public class ReasonServices : IReasonServices
    {
        private readonly IReasonRepository _reasonRepository;

        public ReasonServices(IReasonRepository reasonRepository)
        {
            _reasonRepository = reasonRepository;
        }

        public async Task<Reason> AddAsync(Reason reason)
        {
            return await _reasonRepository.AddAsync(reason);
        }
        public async Task<Reason> GetByIdAsync(int id)
        {
            return await _reasonRepository.GetByIdAsync(id);
        }
        public async Task<Reason> DeleteAsync(int reasonID)
        {
            return await _reasonRepository.DeleteAsync(reasonID);
        }
        public async Task<Reason> UpdateAsync(int id, UpdateReasonModel reason)
        {
            return await _reasonRepository.UpdateAsync(id, reason);
        }
        public async Task<List<Reason>> GetReasonsByUsernameAsync(string username, string reasonContent = null, string courseTitle = null)
        {
            return await _reasonRepository.GetReasonsByUsernameAsync(username, reasonContent, courseTitle);
        }
    }
}
