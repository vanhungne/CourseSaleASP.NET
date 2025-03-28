namespace Project_Cursus_Group3.Data.ViewModels
{
    public class CourseViewModel
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
        public int TotalEnrollment { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public string ShortDescription { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
