using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Bookmark
    {
        [Key]
        public int BookmarkId { get; set; }
        public string? UserName { get; set; }
        [ForeignKey("UserName")]
        public User? User { get; set; }

        public virtual ICollection<BookmarkDetail> BookmarkDetails { get; set; }
    }
}
