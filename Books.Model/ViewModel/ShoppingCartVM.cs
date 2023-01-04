using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Model.ViewModel
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
        public int TotalItems { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
