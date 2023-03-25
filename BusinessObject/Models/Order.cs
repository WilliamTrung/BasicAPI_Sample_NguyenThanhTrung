using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [ForeignKey(nameof(Member))]
        public int MemberId { get; set; }        
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        [Required]
        public DateTime? ShippedDate { get; set; }
        [MinLength(5)]
        public string? Freight { get; set; } 
        public virtual Member? Member { get; set; } 
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
