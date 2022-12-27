using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Model
{
    public class ShoppingCart
    {
        public Product Product { get; set; }
        [Range(1, 100, ErrorMessage = "Please enter the value from 1 to 1000")]
        public int Count { get; set; }
    }
}
