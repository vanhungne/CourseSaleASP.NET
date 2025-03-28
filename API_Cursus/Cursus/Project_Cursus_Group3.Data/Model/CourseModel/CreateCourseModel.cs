using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.CustomActionFilters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Project_Cursus_Group3.Data.Model.CourseModel
{
    public class CreateCourseModel
    {
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Course title is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Course title cannot contain special characters.")]
        public string CourseTitle { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Description cannot contain special characters.")]
        public string Description { get; set; }

        [Range(1, 100, ErrorMessage = "Discount must be a number between 1 and 100.")]
        public double? Discount { get; set; }
        [Range(1, 3, ErrorMessage = "Level must be a number between 1 and 3.")]

        [Required(ErrorMessage = "Level is required.")]
        public int Level { get; set; }

        public bool IsComment { get; set; }

        [Required(ErrorMessage = "Total enrollment is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Total enrollment must be a positive number.")]
        public int TotalEnrollment = 0;

        [Required(ErrorMessage = "Create date is required.")]
        public DateTime? CreatedDate = DateTime.UtcNow.AddHours(7);

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"^(Active|Inactivate|Rejected|Pending|Queuing)$", ErrorMessage = "Status must be one of the following: Active, Inactivate, Rejected, Pending, Queuing.")]
        public string Status = "Pending";

        [Required(ErrorMessage = "Image file is required.")]

        [AllowedExtensions(new string[]
    { ".jpg", ".jpeg", ".jpd", ".png" , ".pdf", ".fjif", ".svg", ".webp" })]
        public IFormFile Image { get; set; }


        [Required(ErrorMessage = "Price is required.")]
        [Range(10, double.MaxValue, ErrorMessage = "Price must be at least $10.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price must be a valid number and cannot contain special characters.")]
        public double Price { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Short description cannot contain special characters.")]
        public string? ShortDescription { get; set; }
    }
}
