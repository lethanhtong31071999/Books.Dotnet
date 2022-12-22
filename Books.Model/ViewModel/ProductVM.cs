using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Model.ViewModel
{
    public class ProductVM
    {
        public Product Product { get;set;}
        [ValidateNever]
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CoverTypeSelectList { get; set; }
    }
}
