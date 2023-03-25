using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        Task UpdateAsync(IEnumerable<OrderDetail> details);
    }
}
