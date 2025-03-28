using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.AboutUsModel
{
    public class CreateAboutUs
    {
        //public int Id { get; set; }
        public string Mission { get; set; }
        public string Vision { get; set; }
        public string CoreValues { get; set; }
        public string PlatformDescription { get; set; }
        public string HowItWorks { get; set; }
        public string RevenueModel { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public DateTime lastUpdated { get; set; }
    }
}
