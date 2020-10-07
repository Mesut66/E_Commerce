using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Ticaret.Business.Abstarct;
using E_Ticaret.Entities;
using E_Ticaret.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Ticaret.WebUI.Controllers
{
    public class ShopController : Controller
    {
        private IProductService _productService;

        public ShopController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult List(string category,int page=1)
        {
            const int pageSize = 3; //her sayfada 4 tane ürün gelecek
            var model = new ProductListModel
            {
                PageInfo = new PageInfo
                {
                    TotalItems = _productService.GetCountBycategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory=category
                },
                Products = _productService.GetProductsByCategory(category,page,pageSize)
            };
            return View(model);
        }

        public IActionResult Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            Product product = _productService.GetProductDetails((int)id);//int e çevirdim id yi.
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductDetailsModel
            {
                product = product,
                categories = product.ProductCategories.Select(i => i.Category).ToList()
            };

            return View(model);
        }
    }
}
