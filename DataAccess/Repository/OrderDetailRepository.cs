using BusinessObject;
using BusinessObject.Models;
using DataAccess.IRepository;
using DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(eStoreDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
        }

        public async Task UpdateAsync(IEnumerable<OrderDetail> details)
        {
            //details --> adjusted records + new records + deleted records
            var orderId = details.First().OrderId;
            var indb = await Get(predicate: detail => detail.OrderId == orderId);
            
            var adjust = details.Where(d => indb.Any(db => db.ProductId == d.ProductId));
            var add = details.Where(d => !indb.Any(db => db.ProductId == d.ProductId));
            var delete = indb.Where(db => !details.Any(d => d.ProductId == db.ProductId));

            foreach(var detail in adjust)
            {
                await Update(detail);
            }
            foreach(var detail in add)
            {
                await AddAsync(detail);
            }
            foreach (var detail in delete)
            {
                await Delete(detail);
            }

        }
    }
}
