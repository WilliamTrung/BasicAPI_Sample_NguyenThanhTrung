using DataAccess.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IMemberRepository MemberRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderDetailRepository OrderDetailRepository { get; }
    }
}
