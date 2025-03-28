namespace Project_Cursus_Group3.Data.ViewModels.LessonDTO
{
    public class LessonViewModel
    {
        public int LessonId { get; set; }
        public int? ChapterId { get; set; }
        public string? VideoURL { get; set; }
        public int? Duration { get; set; }
        public double? Process { get; set; }
        public string? Description { get; set; }
        public string? LessonTitle { get; set; }
        public string Status { get; set; }
    }
}
