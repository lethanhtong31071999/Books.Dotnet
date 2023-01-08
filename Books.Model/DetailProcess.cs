using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Model
{
    public class DetailProcess
    {
        [Key]
        public int Id { get; set; }
        public int? OrderHeaderId { get; set; }
        public string? UpdatedById { get; set; }
        public string? ProcessName { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ValidateNever]
        [ForeignKey("UpdatedById")]
        public User User { get; set; }
        [ValidateNever]
        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; }
    }
}
