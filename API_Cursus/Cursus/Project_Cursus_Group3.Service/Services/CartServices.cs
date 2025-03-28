using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.CartModel;
using Project_Cursus_Group3.Data.ViewModels.CartDTO;
using Project_Cursus_Group3.Data.ViewModels.OrderDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Services
{
    public class CartServices : ICartServices
    {
        private readonly ICartRepository cartRepository;

        public CartServices(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }
        public async Task<CartViewModel> AddToCartAsync(string userName, int courseId)
        {
                 return await cartRepository.AddToCartAsync(userName, courseId);
        }

        public Task<OrderViewModel> CheckoutAsync(string userName)
        {
            return cartRepository.CheckoutAsync(userName);
        }

        public void ClearAllCart(string userName)
        {
            cartRepository.ClearAllCart(userName);
        }

        public CartSummaryViewModel GetCart(string userName)
        {
            return cartRepository.GetCart(userName);
        }

        public void RemoveItemsInCart(string userName, int courseId)
        {
            cartRepository.RemoveItemsInCart(userName, courseId);
        }
    }
}
