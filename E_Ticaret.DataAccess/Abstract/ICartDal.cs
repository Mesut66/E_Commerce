using E_Ticaret.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Ticaret.DataAccess.Abstract
{
    public interface ICartDal : IRepository<Cart>
    {
        Cart GetByUSerId(string userId);
        void DeleteFromCart(int cartId, int productId);
        void ClearCart(string cartId);
    }
}
