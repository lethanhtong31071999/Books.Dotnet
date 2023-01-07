using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Model.PaginationModel
{
    public class PaginatedOrderHeader : Pagination<OrderHeader>
    {
        public string Status { get; set; }
        public bool IsCustomer { get; set; }
        public string CustomerId { get; set; }
    }
}
