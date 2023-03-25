using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Weight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitsInStock { get; set; }
        public virtual Category? Category { get; set; }
    }
}
