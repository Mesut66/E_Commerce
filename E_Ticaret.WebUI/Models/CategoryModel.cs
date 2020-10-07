using E_Ticaret.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Ticaret.WebUI.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Lütfen Kategori Adı giriniz.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Ürün Adı  5-50 karakter arası olmalıdır.")]
        public string Name { get; set; }

        public List<Product> products { get; set; }
    }
}
