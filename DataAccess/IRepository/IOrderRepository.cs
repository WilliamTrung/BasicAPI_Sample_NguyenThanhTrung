using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        decimal Total(Order order);
    }
}
