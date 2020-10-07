using System.Collections.Generic;
using System.Linq;
using E_Ticaret.Business.Abstarct;
using E_Ticaret.DataAccess.Abstract;
using E_Ticaret.Entities;

namespace E_Ticaret.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductDal _productDal;

        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }


        public Product GetById(int id)
        {
            return _productDal.GetById(id);
        }

        public List<Product> GetAll()//Listeleme yaptım
        {
            return _productDal.GetAll().ToList();
        }

        public void Create(Product entity)
        {
            _productDal.Create(entity);
        }

        public void Update(Product entity)
        {
            _productDal.Update(entity);
        }

        public void Delete(Product entity)
        {
            _productDal.Delete(entity);
        }

        public int GetCountBycategory(string category)
        {
            return _productDal.GetCountBycategory(category);
        }

        public Product GetProductDetails(int id)
        {
            return _productDal.GetProductDetails(id);
        }

        public List<Product> GetProductsByCategory(string category,int page,int pageSize)
        {
            return _productDal.GetProductsByCategory(category,page, pageSize);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productDal.GetByIdWithCategories(id);
        }

        public void Update(Product model, int[] categoryIds)
        {
            _productDal.Update(model, categoryIds);
        }
    }
}
