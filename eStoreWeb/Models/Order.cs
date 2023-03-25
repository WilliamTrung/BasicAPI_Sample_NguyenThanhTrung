using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eStoreWeb.Models
{
    public partial class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Freight { get; set; } = null!;

        public virtual Member? Member { get; set; } = null;
        [NotMapped]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = null!;
    }
}
