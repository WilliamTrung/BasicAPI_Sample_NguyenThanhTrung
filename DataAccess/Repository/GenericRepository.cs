using BusinessObject;
using DataAccess.DAO;
using DataAccess.IRepository;
using DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class GenericRepository<TEntity> : GenericDAO<TEntity>, IGenericRepository<TEntity> where TEntity : class
    {
        public GenericRepository(eStoreDbContext context) : base(context)
        {
        }

        public virtual Task<TEntity?> GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
