using System.ComponentModel.DataAnnotations;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        public string? RoleName { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
