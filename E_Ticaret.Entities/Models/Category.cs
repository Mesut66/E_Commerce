using System;
using System.Collections.Generic;
using System.Text;
using E_Ticaret.Entities.Models;

namespace E_Ticaret.Entities
{
   public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }

    }
}
