using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels.ChapterDTO
{
    public class ChapterViewModel
    {
        public int? CourseId { get; set; }
        [Required]
        [RegularExpression(@"^[^!@#$%^&*()_+={}\[\]:;\""'<>,.?/]*$", ErrorMessage = "ChapterTitle cannot contain special characters.")]
        public string? ChapterTitle { get; set; }

        [RegularExpression(@"^[^!@#$%^&*()_+={}\[\]:;\""'<>,.?/]*$", ErrorMessage = "SubDescription cannot contain special characters.")]
        public string? SubDescription { get; set; }

        [RegularExpression(@"^[^!@#$%^&*()_+={}\[\]:;\""'<>,.?/]*$", ErrorMessage = "Description cannot contain special characters.")]
        public string? Description { get; set; }

        [Range(0, 100, ErrorMessage = "Process must be between 0 and 100%.")]
        public double? Process { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be in minutes.")]
        public int? Duration { get; set; }

        [Required]
        [RegularExpression(@"^(Active|Inactive)$", ErrorMessage = "Status must be either Active or Inactive.")]
        public string? Status { get; set; }
        public Course Course { get; set; }
    }
}
