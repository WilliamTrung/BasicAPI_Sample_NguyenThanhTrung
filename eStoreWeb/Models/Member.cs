using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eStoreWeb.Models
{
    public partial class Member
    {
        public Member()
        {
            Orders = new HashSet<Order>();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberId { get; set; }
        public string Email { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
