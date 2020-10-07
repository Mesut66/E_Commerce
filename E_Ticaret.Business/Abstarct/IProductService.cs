 using System;
using System.Collections.Generic;
using System.Text;
using E_Ticaret.Entities;

namespace E_Ticaret.Business.Abstarct
{
   public interface IProductService
   {
       Product GetById(int id);
       Product GetProductDetails(int id);

        List<Product> GetAll();
        List<Product> GetProductsByCategory(string category,int page,int pageSize);


        void Create(Product entity);
       void Update(Product entity);
       void Delete(Product entity);


       int GetCountBycategory(string category);
        Product GetByIdWithCategories(int id);
        void Update(Product model, int[] categoryIds);
    }
}
