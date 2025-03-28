using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.FeedBackModel
{
    public class AddFeedbackModel
    {
        [Range(1, 5, ErrorMessage = "Star must be between 1 and 5.")]
        public int Star { get; set; }

        [Required]
        [RegularExpression(@"^[^!@#$%^&*()_+={}\[\]:;\""'<>,.?/]*$", ErrorMessage = "Content cannot contain special characters.")]
        public string Content { get; set; }

        public int CourseId { get; set; }

    }
}
