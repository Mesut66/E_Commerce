using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using E_Ticaret.Entities;


namespace E_Ticaret.DataAccess.Abstract
{
    public interface ICategoryDal : IRepository<Category>//T: olan yere Product verdik
    {
        Category GetByIdWithProducts(int id);
        void DeleteFromCategory(int categoryId, int productId);
    }
}
