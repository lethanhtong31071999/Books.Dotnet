using Books.Model.PaginationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Model
{
    public class Pagination<T> where T : class
    {
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public int Draw { get; set; }
        public Filter Filter { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
