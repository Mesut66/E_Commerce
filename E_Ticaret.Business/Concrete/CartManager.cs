using E_Ticaret.Business.Abstarct;
using E_Ticaret.DataAccess.Abstract;
using E_Ticaret.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Ticaret.Business.Concrete
{
    public class CartManager : ICartService
    {
        private ICartDal _cartDal;
        public CartManager(ICartDal cartDal)
        {
            _cartDal = cartDal;
        }

        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId);//önce userId yi aldım
            if (cart != null)
            {
                var index = cart.CartItems.FindIndex(i => i.ProductId == productId);
                if (index<0)
                {
                    cart.CartItems.Add(new CartItem()
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        CartId = cart.Id
                    });
                }
                else//ürün varsa üzerine ekleme yapacak
                {
                    cart.CartItems[index].Quantity += quantity;
                }

                _cartDal.Update(cart);//sepet güncellenmiş olacak
            }
        }

        public void ClearCart(string cartId)
        {
            _cartDal.ClearCart(cartId);
        }

        public void DeleteFromCart(string userId, int productId)
        {
            var cart = GetCartByUserId(userId);//önce userId yi aldım
            if (cart != null)
            {
                _cartDal.DeleteFromCart(cart.Id, productId);
            }
        }

        public Cart GetCartByUserId(string userId)
        {
            return _cartDal.GetByUSerId(userId);
        }

        public void InitializeCart(string userId)
        {
            _cartDal.Create(new Cart() { UserId = userId });//User ıd ye göre seper oluştu
        }
    }
}
