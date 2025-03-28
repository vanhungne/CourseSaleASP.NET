using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Certifications
    {
        [Key]
        public int CertificationId { get; set; }
        public string CertificationName { get; set; }
        public string Certification { get; set; }
        public string username { get; set; }
        [ForeignKey("username")]
        public User User { get; set; }
    }
}
