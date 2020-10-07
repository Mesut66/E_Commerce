using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Ticaret.Business.Abstarct;
using E_Ticaret.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Ticaret.WebUI.Controllers
{
   
    public class HomeController : Controller
    {
        private IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            //var model = _productService.GetAll();
            var model = new ProductListModel
            {
                Products = _productService.GetAll()
            };
            return View(model);
        }
    }
}
