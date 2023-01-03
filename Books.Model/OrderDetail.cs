using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Model
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public int Count { get; set; }
        public double FinalPrice { get; set; }

        // Include
        [ValidateNever]
        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; }
        [ValidateNever]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
