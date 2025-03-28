using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.CategoryModel
{
    public class UpdateCategoryModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "CategoryName least 3 character")]
        public string? CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }

    }
}


