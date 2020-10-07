using E_Ticaret.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Ticaret.Business.Abstarct
{
   public interface IOrderService
    {
        void Create(Order entity);
        List<Order> GetOrders(string userId);
    }
}
