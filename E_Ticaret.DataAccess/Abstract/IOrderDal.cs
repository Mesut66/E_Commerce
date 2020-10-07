using System;
using System.Collections.Generic;
using System.Text;
using E_Ticaret.Entities.Models;


namespace E_Ticaret.DataAccess.Abstract
{
    public interface IOrderDal : IRepository<Order>//T: olan yere Order verdik
    {
        List<Order> GetOrders(string userId);
    }
}
