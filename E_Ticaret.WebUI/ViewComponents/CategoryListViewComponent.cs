using E_Ticaret.Business.Abstarct;
using E_Ticaret.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Ticaret.WebUI.ViewComponents
{
    public class CategoryListViewComponent: ViewComponent
    {
        private ICategoryService _categoryService;

        public CategoryListViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IViewComponentResult Invoke()
        {
            var model = new CategoryListViewModel()
            {
                SelectedCategory =RouteData.Values["category"]?.ToString(),//? dememin sebebi category null değilse  string e çevirecek. Null ise stringe çevirmeye gerek kalmayacak. ? yazmazsak olmayan kategory i string e çevirmeye çalışacak ve hata verir
                Categories = _categoryService.GetAll()
            };
            return View(model);
        }


    }
}
