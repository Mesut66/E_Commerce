using E_Ticaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Ticaret.WebUI.Models
{
    public class ProductDetailsModel
    {
        public Product product { get; set; }
        public List<Category> categories { get; set; }
    }
}
