using Project_Cursus_Group3.Data.Model.CartModel;
using Project_Cursus_Group3.Data.ViewModels.CartDTO;
using Project_Cursus_Group3.Data.ViewModels.OrderDTO;
using System.Collections.Generic;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface ICartRepository
    {
        Task<CartViewModel> AddToCartAsync(string userId, int courseId);
        CartSummaryViewModel GetCart(string userId);
        void ClearAllCart(string userId);
        void RemoveItemsInCart(string userId, int courseId);
        Task<OrderViewModel> CheckoutAsync(string userName);
    }
}
