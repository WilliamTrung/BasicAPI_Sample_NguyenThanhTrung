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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductRepository(eStoreDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _unitOfWork = unitOfWork;
        }
        public override async Task<Product?> GetById(int id)
        {
            var find = await _unitOfWork.ProductRepository.Get(predicate: p => p.ProductId == id);
            var found = find.FirstOrDefault();
            return found;
        }
    }
}
