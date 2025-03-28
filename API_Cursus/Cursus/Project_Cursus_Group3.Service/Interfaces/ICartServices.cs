using Project_Cursus_Group3.Data.Model.CartModel;
using Project_Cursus_Group3.Data.ViewModels.CartDTO;
using Project_Cursus_Group3.Data.ViewModels.OrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface ICartServices
    {
        Task<CartViewModel> AddToCartAsync(string userName, int courseId);
        CartSummaryViewModel GetCart(string userId);
        void ClearAllCart(string userName);
        void RemoveItemsInCart(string userName, int courseId);
        Task<OrderViewModel> CheckoutAsync(string userName);
    }
}
