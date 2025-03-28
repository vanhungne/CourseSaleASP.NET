using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels.FeedbackDTO
{
    public class FeedbackViewModel
    {
        [Range(1, 5, ErrorMessage = "Star must be between 1 and 5.")]
        public int? Star { get; set; }

        [Required]
        [RegularExpression(@"^[^!@#$%^&*()_+={}\[\]:;\""'<>,.?/]*$", ErrorMessage = "Content cannot contain special characters.")]
        public string Content { get; set; }

        public string Attachment { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [RegularExpression(@"^(Accept|Reject)$", ErrorMessage = "Status must be either Accept or Reject.")]
        public string Status { get; set; }
        public int? CourseId { get; set; }

        [Required]
        [MinLength(5)]
        [RegularExpression(@"^[^!@#$%^&*()+={}\[\]:;\""'<>,.?/]*$", ErrorMessage = "UserName cannot contain special characters.")]
        public string UserName { get; set; }
        public Course course { get; set; }
    }
}
