using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class OrderDetail
    {
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Discount { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
