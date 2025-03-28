using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class User
    {
        [Key]
        public string? UserName { get; set; }
        public int RoleId { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Address { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedDate { get; set; }
        public string? Avatar { get; set; }

        //public string? Certification { get; set; }
    
        [Required]
        public DateTime DOB { get; set; }
        public string? adminComment { get; set; }
        public string? deleteComment { get; set; }

        [Required]
        public string? Status { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
        public Boolean? isVerify { get; set; }
        public virtual ICollection<PurchasedCourse>? PurchasedCourses { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<QuizAttemp>? QuizAttemps { get; set; }
        public virtual Wallet? Wallet { get; set; }
        public virtual Bookmark? Bookmark { get; set; }
        public virtual ICollection<Certifications> Certifications { get; set; }
    }
}
