using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Model.PaginationModel
{
    public class Filter
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public string TextSearch { get; set; }
    }
}
