using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using E_Ticaret.Entities;


namespace E_Ticaret.DataAccess.Abstract
{
    public interface IProductDal : IRepository<Product>
    {
        List<Product> GetProductsByCategory(string category,int page,int pageSize);

        Product GetProductDetails(int id);
        int GetCountBycategory(string category);
        Product GetByIdWithCategories(int id);
        void Update(Product model, int[] categoryIds);
    }
}
