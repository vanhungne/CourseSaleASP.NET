using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.UserModel
{
    public class UserProfileUpdateModel
    {

        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [RegularExpression(@"^[\w-\.]+@(gmail\.com|fpt\.edu\.vn)$", ErrorMessage = "Email must be a valid Gmail or FPT email address.")]

        public string? Email { get; set; }
      
        [StringLength(100, ErrorMessage = "Address must be at most 100 characters long.")]
        [RegularExpression(@"^[^!@#$%^&*()_+=\[{\]};:<>|?-]*$", ErrorMessage = "Address must not contain special characters.")]

        public string? Address { get; set; } = string.Empty;
        //[MinLength(10, ErrorMessage = "Full name must be at least 10 characters long.")]
        //[MaxLength(200, ErrorMessage = "Full name max 200 characters long.")]
        [RegularExpression(@"^[^!@#$%^&*()_+=\[{\]};:<>|./?,-]*$", ErrorMessage = "Full name must not contain special characters.")]
        public string? FullName { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
        public IFormFile? Certification { get; set; }

        public string Status = "Active";
        public DateTime DOB { get; set; }
    }
}
