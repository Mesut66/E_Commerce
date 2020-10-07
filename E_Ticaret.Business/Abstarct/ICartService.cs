using E_Ticaret.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Ticaret.Business.Abstarct
{
    public interface ICartService
    {
        void InitializeCart(string userId);
        Cart GetCartByUserId(string userId);

        void AddToCart(string userId, int productId, int quantity);
        void DeleteFromCart(string userId, int productId);
        void ClearCart(string cartId);
    }
}
