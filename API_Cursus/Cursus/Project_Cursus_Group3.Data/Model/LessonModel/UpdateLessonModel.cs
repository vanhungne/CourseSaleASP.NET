using Microsoft.AspNetCore.Http;

namespace Project_Cursus_Group3.Data.Model.LessonModel
{
    public class UpdateLessonModel
    {
        public int? ChapterId { get; set; }
        public IFormFile? VideoURL { get; set; }
        public int? Duration { get; set; }
        public double? Process { get; set; }
        public string? Description { get; set; }
        public string? LessonTitle { get; set; }
    }
}
