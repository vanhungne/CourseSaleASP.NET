using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model
{
    public class CreateCategoryModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "CategoryName least 3 character")]
        public string? CategoryName { get; set; }
        public string? Status = "Active";

        public int? ParentCategoryId { get; set; }
    }
}
