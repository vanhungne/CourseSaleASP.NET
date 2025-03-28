using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.Report
{
    public class UpdateReportModel
    {
        [StringLength(100, ErrorMessage = "Issue cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Issue cannot contain special characters.")]
        public string? Issue { get; set; }

       
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.!?]*$", ErrorMessage = "Content cannot contain special characters.")]
        public string? Content { get; set; }

        //[RegularExpression(@"^(Accept|Reject)$", ErrorMessage = "Status must be either 'Accept', 'Reject', or 'Pending'.")]
        //public string? Status { get; set; }

    }
}
