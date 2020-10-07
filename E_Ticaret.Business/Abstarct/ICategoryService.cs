using System;
using System.Collections.Generic;
using System.Text;
using E_Ticaret.Entities;

namespace E_Ticaret.Business.Abstarct
{
   public interface ICategoryService
    {
        Category GetById(int id);
        Category GetByIdWithProducts(int id);
        List<Category> GetAll();

        void Create(Category entity);
        void Update(Category entity);
        void Delete(Category entity);
        void DeleteFromCategory(int categoryId, int productId);
    }
}
