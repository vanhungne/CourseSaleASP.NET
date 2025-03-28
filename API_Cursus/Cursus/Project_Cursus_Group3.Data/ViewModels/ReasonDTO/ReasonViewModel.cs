using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels.ReasonDTO
{
    public class ReasonViewModel
    {
        public string message { get; set; }

        public int ReasonId { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
        public int? CourseId { get; set; }
        public Course Course { get; set; }
    }
}
