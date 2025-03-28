using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

using Project_Cursus_Group3.Data.CustomActionFilters;

public class UpdateCourseModel
{
    //[Required(ErrorMessage = "Course must have a title!")]
    [MaxLength(100, ErrorMessage = "Title max 100 characters long.")]
    [RegularExpression(@"^[a-zA-Z0-9\sÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẮẰẲẴẶắằẳẵặƯứừửữự]+$", ErrorMessage = "Course Title must not contain special characters.")]
    public string? CourseTitle { get; set; }

    //[Required(ErrorMessage = "CategoryId is required!")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
    public int? CategoryId { get; set; }

    //[Required(ErrorMessage = "Description is required!")]
    [MaxLength(1000, ErrorMessage = "Description max 1000 characters long.")]
    [RegularExpression(@"^[a-zA-Z0-9\sÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẮẰẲẴẶắằẳẵặƯứừửữự]+$", ErrorMessage = "Description must not contain special characters.")]
    public string? Description { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public double? Discount { get; set; }

    //[Required(ErrorMessage = "Course must have a level!")]
    [Range(1, 3, ErrorMessage = "Level must be 1, 2, or 3.")]
    public int? Level { get; set; }

    public DateTime CreatedDate =  DateTime.UtcNow.AddHours(7);

    public bool? IsComment { get; set; }

    [AllowedExtensions(new string[]
    { ".jpg", ".jpeg", ".jpd", ".png" , ".pdf", ".fjif", ".svg", ".webp" })]
    public IFormFile? Image { get; set; }

    //[Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be higher or equal to 0!")]
    public double? Price { get; set; }

    //[Required]
    [RegularExpression(@"^[a-zA-Z0-9\sÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẮẰẲẴẶắằẳẵặƯứừửữự]+$", ErrorMessage = "Summary must not contain special characters.")]
    [MaxLength(200, ErrorMessage = "Summary max 200 characters long.")]
    public string? ShortDescription { get; set; }
}
