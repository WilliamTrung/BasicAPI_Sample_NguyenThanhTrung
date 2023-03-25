using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eStoreWeb.Models
{
    public partial class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Weight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitsInStock { get; set; }

        public virtual Category? Category { get; set; } = null;
    }
}
