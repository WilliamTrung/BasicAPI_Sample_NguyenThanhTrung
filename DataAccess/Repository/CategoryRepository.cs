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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryRepository(eStoreDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Category?> GetById(int id)
        {
            var find = await _unitOfWork.CategoryRepository.Get(predicate: c => c.CategoryId == id);
            var found = find.FirstOrDefault();
            return found;
        }
    }
}
