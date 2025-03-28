using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Project_Cursus_Group3.Data.Repository
{
    public class UploadFile
    {
        private readonly IConfiguration _configuration;
        public UploadFile(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> UploadFileAsync(IFormFile file, string fileType)
        {
            if (file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var bucket = _configuration["FireBase:Bucket"];

                string folderName = fileType == "certification" ? "certifications" : "avatars";

                var task = new FirebaseStorage(bucket)
                    .Child(folderName)
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await task;

                return downloadUrl;
            }
            return null;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var bucket = _configuration["FireBase:Bucket"];

                var task = new FirebaseStorage(bucket)
                    .Child("certifications")
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await task;

                return downloadUrl;
            }
            return null;
        }

    }
}
