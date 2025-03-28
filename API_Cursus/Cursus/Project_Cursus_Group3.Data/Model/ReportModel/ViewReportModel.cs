using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels.ReportDTO
{
    public class ViewReportModel
    {
        public int ReportId { get; set; }
        public string? Issue { get; set; }
        public string? Attachment { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
        public int? CourseId { get; set; }
        public string? UserName { get; set; }
        public string? CourseTitle { get; set;}
    }
}
