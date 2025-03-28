﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model
{
    public class ResubmitCourseModel
    {
        [Required]
        [MaxLength(500, ErrorMessage = "Reason max 500 characters long.")]
        [RegularExpression(@"^[^!@#$%^&*()_+=\[{\]};:<>|./?,-]*$", ErrorMessage = "Reason must not contain special characters.")]
        public string reasonContent { get; set; }
    }
}
