using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.ViewModels.UserDTO;

namespace Project_Cursus_Group3.Data.ViewModels.CourseDTO
{
    public class CourseViewGET
    {
        public UserViewModel? User { get; set; }
        public CategoryViewModel? Category { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
        public double? Discount { get; set; }
        public int? Level { get; set; }
        public bool IsComment { get; set; }
        public int? TotalEnrollment { get; set; }
        public string? Status { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }
        public string? ShortDescription { get; set; }
        public double? AverageStarRating { get; set; }


    }
}
