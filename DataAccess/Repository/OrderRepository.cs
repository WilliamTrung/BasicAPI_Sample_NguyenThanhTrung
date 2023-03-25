using BusinessObject;
using BusinessObject.Models;
using DataAccess.IRepository;
using DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly IUnitOfWork _unitOfWork;
       

        public OrderRepository(eStoreDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _unitOfWork= unitOfWork;
        }
        public override async Task<IEnumerable<Order>> Get(Expression<Func<Order, bool>>? predicate = null, string? includeProperties = null)
        {
            var result =  base.Get(predicate, includeProperties);
            //get detail start
            foreach(var order in result.Result)
            {
                var details = await _unitOfWork.OrderDetailRepository.Get(predicate: d => d.OrderId == order.OrderId, includeProperties: "Product");
                order.OrderDetails = details.ToList();
            }
            //get detail end
            return result.Result;
        }
        public override async Task AddAsync(Order entity)
        {
            if(entity.ShippedDate == null)
            {
                entity.ShippedDate = entity.RequiredDate;
            }
            if(entity.RequiredDate < entity.OrderDate)
            {
                entity.RequiredDate = DateTime.Today;
            }
            List<OrderDetail> details = entity.OrderDetails.ToList();
            foreach (var detail in entity.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetById(detail.ProductId);
                detail.Product = null;
                if(detail.Discount == null)
                {
                    detail.Discount = 0;
                }
                if (product != null)
                    detail.UnitPrice = product.UnitPrice;
                else
                    throw new Exception("Product is invalid! ID: " + detail.ProductId);
            }
            await base.AddAsync(entity);
            // add successfully
            foreach(var detail in entity.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetById(detail.ProductId);
                if(product != null)
                {
                    product.UnitsInStock -= detail.Quantity;
                    await _unitOfWork.ProductRepository.Update(product);
                }
            }
        }
        public override async Task Update(Order entity)
        {
            var details = entity.OrderDetails;
            if (details != null)
                await _unitOfWork.OrderDetailRepository.UpdateAsync(details);
            entity.OrderDetails = null;
            await base.Update(entity);
        }
        public override async Task<Order?> GetById(int id)
        {
            var find = await Get(predicate: order => order.OrderId== id);
            var found = find.FirstOrDefault();
            return found;
        }

        public decimal Total(Order order)
        {
            decimal result = 0;
            if (order.OrderDetails != null)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var calc = detail.UnitPrice * detail.Quantity;
                    if (detail.Discount != null)
                    {
                        calc -= calc * (decimal)detail.Discount;
                    }
                    result += calc;    
                }
            }
            return result;
        }
    }
}
