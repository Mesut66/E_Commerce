using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Ticaret.DataAccess.Abstract;
using E_Ticaret.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Ticaret.DataAccess.Concrete.EfCore
{
    public class EfCoreOrderDal : EfCoreRepository<Order, TicaretContext>, IOrderDal
    {
        public List<Order> GetOrders(string userId)
        {
            using (var context = new TicaretContext())
            {
                var orders = context.Orders
                    .Include(i => i.OrderItems)
                    .ThenInclude(i => i.Product)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(userId))//yukardan gelen userId varsa!!
                {
                    orders = orders.Where(i => i.UserId == userId);
                }

                return orders.ToList();
            }
        }
    }
}
