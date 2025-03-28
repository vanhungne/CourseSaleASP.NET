using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.CertificationModel
{
    public class UpdateCertification
    {
        public string CertificationName { get; set; }
        public IFormFile? Certification { get; set; }
    }
}
