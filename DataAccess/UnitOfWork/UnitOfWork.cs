using BusinessObject;
using DataAccess.IRepository;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly eStoreDbContext _context;
        public UnitOfWork(eStoreDbContext context)
        {
            _context= context;
            Init();
        }

        public IProductRepository ProductRepository { get; private set; }

        public ICategoryRepository CategoryRepository { get; private set; }

        public IMemberRepository MemberRepository { get; private set; }

        public IOrderRepository OrderRepository { get; private set; }

        public IOrderDetailRepository OrderDetailRepository { get; private set; }

        void Init()
        {
            ProductRepository = new ProductRepository(_context, this);
            CategoryRepository = new CategoryRepository(_context, this);
            OrderDetailRepository= new OrderDetailRepository(_context, this);
            OrderRepository = new OrderRepository(_context, this);
            MemberRepository = new MemberRepository(_context, this);
        }
    }
}
