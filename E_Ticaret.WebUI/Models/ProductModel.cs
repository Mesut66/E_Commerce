using E_Ticaret.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Ticaret.WebUI.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Lütfen Ürün Adı giriniz.")]
        [StringLength(50,MinimumLength =5,ErrorMessage ="Ürün Adı  5-50 karakter arası olmalıdır.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Lütfen Resim Ekleyiniz.")]
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Lütfen Açıklama giriniz")]
        [StringLength(100, MinimumLength = 15, ErrorMessage = "Ürün Açıklaması  15-100 karakter arası olmalıdır.")]
        public string Description { get; set; }
        [Required(ErrorMessage ="Lütfen Fiyat giriniz")]
        [Range(1000, 10000, ErrorMessage = "Fiyat 2000₺ - 10000₺ arasında olmalıdır")]
        public decimal Price { get; set; }

        public List<Category> SelectedCategories { get; set; }
    }
}
