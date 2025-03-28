
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Project_Cursus_Group3.Data.Model.LoginModel
{
    public class RegisterLoginModel
    {
        [RegularExpression(@"^[a-zA-Z0-9\sÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẮẰẲẴẶắằẳẵặƯứừửữự]+$", ErrorMessage = "UserName must not contain special characters.")]
        [MinLength(5, ErrorMessage = "User name must be at least 5 characters long.")]

        public string UserName { get; set; }
        public int RoleId { get; set; }

        [StringLength(100, ErrorMessage = "Password must be at least 8 characters long", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[a-z])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one number, and one character")]

        public string Password { get; set; }

        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [RegularExpression(@"^[\w-\.]+@(gmail\.com|fpt\.edu\.vn)$", ErrorMessage = "Email must be a valid Gmail or FPT email address.")]

        public string Email { get; set; }
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must  be begin 0 and 10 digits long.")]

        public string PhoneNumber { get; set; }
        [StringLength(100, ErrorMessage = "Address must be at most 100 characters long.")]
        [RegularExpression(@"^[^!@#$%^&*()_+=\[{\]};:<>|?-]*$", ErrorMessage = "Address must not contain special characters.")]

        public string? Address { get; set; } = string.Empty;
        [MinLength(10, ErrorMessage = "Full name must be at least 10 characters long.")]
        [MaxLength(200, ErrorMessage = "Full name max 200 characters long.")]
        [RegularExpression(@"^[^!@#$%^&*()_+=\[{\]};:<>|./?,-]*$", ErrorMessage = "Full name must not contain special characters.")]

        public string? FullName { get; set; } = string.Empty;
        public DateTime CreatedDate = DateTime.UtcNow.AddHours(7);
        public IFormFile? Avatar { get; set; }
        public IFormFile? Certification { get; set; }

        public DateTime DOB { get; set; }
        public string? Comment = string.Empty;
        public string Status = "Pending";
    }
}
