using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public interface IGenericDAO <TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>>? predicate = null, string? includeProperties = null);
    }
}
