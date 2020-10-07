using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using E_Ticaret.Business.Abstarct;
using E_Ticaret.Entities;
using E_Ticaret.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Ticaret.WebUI.Controllers
{
    [Authorize]//Login olmadan buraya giremez
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;

        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult ListProduct()//LİSTELEME
        {
            var model = new ProductListModel
            {
                Products = _productService.GetAll()
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateProduct()//EKLEME
        {
            return View(new ProductModel());
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductModel productModel)
        {

            if (ModelState.IsValid)//Yani bana geçerli bir veri geldiyse
            {
                var model = new Product
                {
                    Name = productModel.Name,
                    Price = productModel.Price,
                    Description = productModel.Description,
                    ImageUrl = productModel.ImageUrl
                };
                _productService.Create(model);
                return RedirectToAction("ListProduct");

            }

            return View(productModel);


        }

        [HttpGet]
        public IActionResult EditProduct(int? id)//GÜNCELLEME
        {

            if (id == null)//id boşsa hata sayfasına gidecek
            {
                return NotFound();
            }

            var model = _productService.GetByIdWithCategories((int)id);// nuul dan int e çevirdik 
            if (model == null)
            {
                return NotFound();
            }
            var pModel = new ProductModel
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                SelectedCategories = model.ProductCategories.Select(i => i.Category).ToList()

            };
            ViewBag.Categories = _categoryService.GetAll();
            return View(pModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel productModel, int[] categoryIds, IFormFile file)
        {

            if (ModelState.IsValid)
            {
                var model = _productService.GetById(productModel.Id);

                if (model == null)
                {
                    return NotFound();
                }
                model.Name = productModel.Name;
                model.Description = productModel.Description;
                //model.ImageUrl = productModel.ImageUrl;
                model.Price = productModel.Price;

                if (file != null)//Resim yükleme kısmı
                {
                    model.ImageUrl = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", file.FileName);
                    using (var stream = new FileStream(path,FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                _productService.Update(model, categoryIds);

                return RedirectToAction("ListProduct");
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View(productModel);

        }

        [HttpPost]
        public IActionResult DeleteProduct(int productId)//Silme 
        {
            var model = _productService.GetById(productId);
            if (model != null)
            {
                _productService.Delete(model);
            }
            return RedirectToAction("ListProduct");
        }


        //Categori işlemleri---------------------------------------------

        public IActionResult ListCategory()//Listeleme
        {
            var model = new CategoryListModel
            {
                categories = _categoryService.GetAll()
            };

            return View(model);
        }

        public IActionResult CreateCategory()//Ekleme
        {

            return View(new CategoryModel());
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel categoryModel)
        {
            if (ModelState.IsValid)//Yani bana geçerli bir veri geldiyse
            {
                var model = new Category
                {
                    Name = categoryModel.Name
                };
                _categoryService.Create(model);
                return RedirectToAction("ListCategory");

            }
            return View(categoryModel);



        }


        public IActionResult EditCategory(int id)//Güncelleme
        {
            var model = _categoryService.GetByIdWithProducts(id);

            var cModel = new CategoryModel
            {
                Id = model.Id,
                Name = model.Name,
                products = model.ProductCategories.Select(i => i.Product).ToList()
            };
            return View(cModel);
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel categoryModel)
        {
            var model = _categoryService.GetById(categoryModel.Id);
            if (model == null)
            {
                return NotFound();
            }

            model.Name = categoryModel.Name;
            _categoryService.Update(model);
            return RedirectToAction("ListCategory");
        }


        [HttpPost]
        public IActionResult DeleteCategory(int categoryId)//Silme
        {
            var model = _categoryService.GetById(categoryId);
            if (model != null)
            {
                _categoryService.Delete(model);
            }
            return RedirectToAction("ListCategory");
        }

        [HttpPost]//Category nin Ürününü silme 
        public IActionResult DeleteFromCategory(int categoryId, int productId)//iki Id yide allıp silecek
        {
            _categoryService.DeleteFromCategory(categoryId, productId);
            return Redirect("/Admin/EditCategory/" + categoryId);
        }

    }
}
