using System;
using System.Collections.Generic;
using System.Text;

namespace E_Ticaret.Entities.Models
{
    public class Cart//sepet
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public List<CartItem> CartItems { get; set; }
    }
}
