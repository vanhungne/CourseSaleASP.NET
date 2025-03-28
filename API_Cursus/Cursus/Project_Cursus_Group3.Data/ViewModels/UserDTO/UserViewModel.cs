using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels.UserDTO
{
    public class UserViewModel
    {
        public string? UserName { get; set; }
        public int RoleId { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Certification { get; set; }
        public DateTime DOB { get; set; }
        public string? Comment { get; set; }
        public string? Status { get; set; }
    }
}
