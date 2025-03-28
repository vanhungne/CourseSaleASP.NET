using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.CertificationModel;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Services
{
    public class CertificationService : ICertificationService
    {
        private readonly ICertificationRepository certificationRepository;

        public CertificationService(ICertificationRepository certificationRepository)
        {
            this.certificationRepository = certificationRepository;
        }

        public Task<Certifications> AddAsync(UpdateCertification certifications, string username)
        {
            return certificationRepository.AddAsync(certifications, username);
        }

        public async Task DeleteAsync(int id)
        {
           await certificationRepository.DeleteAsync(id);
        }
        public Task<Certifications> GetByIdAndUsernameAsync(int id, string username)
        {
            return certificationRepository.GetByIdAndUsernameAsync(id, username);
        }
        public Task<Certifications> UpdateAsync(int id, UpdateCertification certification, string username)
        {
            return certificationRepository.UpdateAsync(id, certification, username);
        }

    }
}
