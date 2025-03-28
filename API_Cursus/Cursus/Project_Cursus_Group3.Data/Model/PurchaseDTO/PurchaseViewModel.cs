using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels.PurchaseDTO
{
    public class PurchaseViewModel
    {
        public CourseViewModel course { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
    }
}
