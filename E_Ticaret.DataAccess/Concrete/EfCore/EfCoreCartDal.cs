using E_Ticaret.DataAccess.Abstract;
using E_Ticaret.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace E_Ticaret.DataAccess.Concrete.EfCore
{
    public class EfCoreCartDal : EfCoreRepository<Cart, TicaretContext>, ICartDal
    {

        public override void Update(Cart entity)//
        {
            //base.Update(entity);
            using (var context = new TicaretContext())
            {
                context.Carts.Update(entity);
                context.SaveChanges();
            }
        }


        public Cart GetByUSerId(string userId)
        {
            using (var context = new TicaretContext())
            {
                return context.Carts
                    .Include(x => x.CartItems)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefault(i => i.UserId == userId);
            }
        }

        public void DeleteFromCart(int cartId, int productId)
        {
            using (var context = new TicaretContext())
            {
                var cmd = @"delete from CartItem where CartId = @p0 and ProductId=@p1";
                context.Database.ExecuteSqlCommand(cmd, cartId, productId);

            }
        }

        public void ClearCart(string cartId)
        {
            using (var context = new TicaretContext())
            {
                var cmd = @"delete from CartItem where CartId = @p0 ";
                context.Database.ExecuteSqlCommand(cmd, cartId);

            }
        }
    }
}
