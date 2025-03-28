using Project_Cursus_Group3.Data.Entities;

namespace Project_Cursus_Group3.Data.ViewModels.CartDTO
{
    public class CartViewModel
    {
        public string Message { get; set; }  

        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }

        public string ShortDescription { get; set; }
        public DateTime AddedDate { get; set; }

      

    }
    public class CartSummaryViewModel
    {
        public List<CartViewModel> CartItems { get; set; } = new List<CartViewModel>();
        public int TotalItems { get; set; }
        public double TotalPrice { get; set; }
    }
}
