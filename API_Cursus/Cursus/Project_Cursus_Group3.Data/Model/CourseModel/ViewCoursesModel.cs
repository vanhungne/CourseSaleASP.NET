using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.CourseModel
{
    public class ViewCoursesModel
    {
        public int CourseId { get; set; }
        public string Username { get; set; }
        public int CategoryId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        public int Level { get; set; }
        public bool IsComment { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalEnrollment { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public string ShortDescription { get; set; }
        public User User { get; set; }
    }
}
