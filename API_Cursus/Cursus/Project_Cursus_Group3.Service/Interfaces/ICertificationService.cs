using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.CertificationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface ICertificationService
    {
        Task<Certifications> GetByIdAndUsernameAsync(int id, string username);
        Task DeleteAsync(int id);
        Task<Certifications> AddAsync(UpdateCertification certifications, string username);
        Task<Certifications> UpdateAsync(int id, UpdateCertification certification, string username);
    }
}
