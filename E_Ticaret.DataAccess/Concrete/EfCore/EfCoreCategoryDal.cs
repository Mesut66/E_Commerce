using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using E_Ticaret.DataAccess.Abstract;
using E_Ticaret.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Ticaret.DataAccess.Concrete.EfCore
{
    public class EfCoreCategoryDal : EfCoreRepository<Category, TicaretContext>, ICategoryDal
    {
        public void DeleteFromCategory(int categoryId, int productId)
        {
            using (var context = new TicaretContext())
            {
                var komut = @"delete from ProductCategory where ProductId = @p0 and CategoryId=@p1";//sql sorgusu yazdık
                context.Database.ExecuteSqlCommand(komut, productId, categoryId);                        
            }
        }

        public Category GetByIdWithProducts(int id)
        {
            using (var context = new TicaretContext())
            {
                var model = context.Categories.Where(i => i.Id == id)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefault();

                return model;
            }
        }
    }
}
