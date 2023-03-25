using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IGenericRepository<TEntity> : IGenericDAO<TEntity> where TEntity : class
    {
        public Task<TEntity?> GetById(int id);
    }
}
