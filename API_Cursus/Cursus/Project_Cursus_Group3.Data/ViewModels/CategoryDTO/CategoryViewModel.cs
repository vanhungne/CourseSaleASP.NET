using Project_Cursus_Group3.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Status { get; set; }
        public int? ParentCategoryId { get; set; }

    }
}
