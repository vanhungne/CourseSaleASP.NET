using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.CertificationModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class CertificationRepository : Repository<Certifications>, ICertificationRepository
    {
        private readonly CursusDbContext _context;
        private readonly IConfiguration _configuration;

        public CertificationRepository(CursusDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Certifications> AddAsync(UpdateCertification certificationModel, string username)
        {
            if (certificationModel == null || certificationModel.Certification == null)
                throw new ArgumentNullException(nameof(certificationModel), "Certification model or file cannot be null.");

            var certificationUrl = await UploadFileAsync(certificationModel.Certification, "certification");
            if (certificationUrl == null)
                throw new InvalidOperationException("Failed to upload certification file.");

            var certification = new Certifications
            {
                CertificationName = certificationModel.CertificationName,
                Certification = certificationUrl,
                username = username
            };

            Entities.Add(certification);
            await _context.SaveChangesAsync();

            return certification;
        }

        public async Task DeleteAsync(int id)
        {
            var existingCertification = await Entities.FirstOrDefaultAsync(c => c.CertificationId == id);

            Entities.Remove(existingCertification);
            await _context.SaveChangesAsync();
        }

        public async Task<Certifications> GetByIdAndUsernameAsync(int id, string username)
        {
            var certification = await Entities.FirstOrDefaultAsync(c => c.CertificationId == id && c.username == username);
            if (certification == null)
                throw new KeyNotFoundException($"Certification with ID {id} not found for the user.");

            return certification;
        }

        public async Task<Certifications> UpdateAsync(int id, UpdateCertification certificationModel, string username)
        {
            var existingCertification = await GetByIdAndUsernameAsync(id, username);
            if (existingCertification == null || certificationModel == null)
                return null;

            existingCertification.CertificationName = certificationModel.CertificationName;

            if (certificationModel.Certification != null)
            {
                var certificationUrl = await UploadFileAsync(certificationModel.Certification, "certification");
                if (!string.IsNullOrEmpty(certificationUrl))
                {
                    existingCertification.Certification = certificationUrl;
                }
            }

            await _context.SaveChangesAsync();
            return existingCertification;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileType)
        {
            if (file == null || file.Length == 0)
                return null;

            var bucket = _configuration["FireBase:Bucket"];
            if (string.IsNullOrEmpty(bucket))
                throw new InvalidOperationException("Firebase bucket configuration is missing.");

            var folderName = fileType == "certification" ? "certifications" : "avatars";

            try
            {
                using var stream = file.OpenReadStream();
                var task = new FirebaseStorage(bucket)
                    .Child(folderName)
                    .Child(file.FileName)
                    .PutAsync(stream);

                return await task;
            }
            catch (Exception ex)
            {
                throw new Exception("File upload failed.", ex);
            }
        }
    }
}
