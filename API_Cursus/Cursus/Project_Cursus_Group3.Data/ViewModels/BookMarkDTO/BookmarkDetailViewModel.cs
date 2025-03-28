namespace Project_Cursus_Group3.Data.ViewModels.BookMarkDTO
{
    public class BookmarkDetailViewModel
    {
        public int BookmarkId { get; set; }
        public string Status { get; set; }
        public CourseViewModel Course { get; set; }
    }
}
