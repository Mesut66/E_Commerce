using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using E_Ticaret.DataAccess.Abstract;
using E_Ticaret.Entities;
using E_Ticaret.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace E_Ticaret.DataAccess.Concrete.EfCore
{
    public class EfCoreProductDal : EfCoreRepository<Product, TicaretContext>, IProductDal
    {
        public Product GetByIdWithCategories(int id)
        {
            using (var context = new TicaretContext())//Burdada ortak alandan category e ulaştım
            {
                var model = context.Products.Where(i => i.Id == id)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Category).FirstOrDefault();

                return model;
            }
        }
        public int GetCountBycategory(string category)
        {
            using (var context = new TicaretContext())
            { 
                var products = context.Products.AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                        .Include(i => i.ProductCategories)
                        .ThenInclude(i => i.Category)
                        .Where(i => i.ProductCategories.Any(a =>
                            a.Category.Name.ToLower() ==category.ToLower())); 

                }

                return products.Count();
            }
        }


        public Product GetProductDetails(int id)
        {
            using (var context = new TicaretContext())
            {
                return context.Products
                    .Where(i => i.Id == id)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Category)
                    .FirstOrDefault();
            }
        }

        public List<Product> GetProductsByCategory(string category,int page,int pageSize)
        {
            using (var context = new TicaretContext())
            {
                var products = context.Products.AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                        .Include(i => i.ProductCategories)
                        .ThenInclude(i => i.Category)
                        .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower())); 

                }

                //return products.ToList();

                return products.Skip((page-1)*pageSize).Take(pageSize).ToList();

            }
        }

        public void Update(Product model, int[] categoryIds)
        {
            using (var context = new TicaretContext())
            {
                var product = context.Products
                    .Include(i => i.ProductCategories)
                    .FirstOrDefault(i => i.Id == model.Id);

                if (product != null)
                {
                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.ImageUrl = model.ImageUrl;
                    product.Price = model.Price;

                    product.ProductCategories = categoryIds.Select(i => new ProductCategory
                    {
                        CategoryId = i,
                        ProductId = model.Id
                    }).ToList();

                    context.SaveChanges();
                }                              
            }
        }
    }
}
