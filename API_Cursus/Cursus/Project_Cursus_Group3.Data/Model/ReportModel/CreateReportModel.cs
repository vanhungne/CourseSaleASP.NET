using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.ReportModel
{
    public class CreateReportModel
    {

        [Required(ErrorMessage = "Issue is required.")]
        [StringLength(100, ErrorMessage = "Issue cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Issue cannot contain special characters.")]
        public string Issue { get; set; }


        [Required(ErrorMessage = "Content is required.")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.!?]*$", ErrorMessage = "Content cannot contain special characters.")]
        public string Content { get; set; }

        //[Required(ErrorMessage = "Status is required.")]
        //[RegularExpression(@"^(Accept|Reject)$", ErrorMessage = "Status must be either 'Accept' or 'Reject'.")]
        //public string Status { get; set; }

        [Required(ErrorMessage = "CourseId is required.")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Username cannot contain special characters.")]
        public string UserName { get; set; }
    }
}
