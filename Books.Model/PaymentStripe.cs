using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Model
{
    public class PaymentStripe
    {
        public Product Product { get; set; }
        public int Count { get; set; }
        public double FinalPrice { get; set; }
    }
}
