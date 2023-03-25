using BusinessObject;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class GenericDAO<TEntity> : IGenericDAO<TEntity> where TEntity : class
    {
        private readonly eStoreDbContext _context;
        private DbSet<TEntity> _entities;
        public GenericDAO(eStoreDbContext context)
        {
            _context = context;
            _entities= _context.Set<TEntity>();
        }
        public virtual async Task AddAsync(TEntity entity)
        {
            _entities.Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Delete(TEntity entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>>? predicate = null, string? includeProperties = null)
        {
            IQueryable<TEntity>? query = _entities.AsQueryable();
            query = predicate == null ? query : query.Where(predicate);
            if (includeProperties != null)
            {
                foreach(var property in includeProperties.Split(","))
                {
                    query = query.Include(property);
                }
            }
            
            return await query.ToListAsync();
        }

        public virtual async Task Update(TEntity entity)
        {
            //_entities.Update(entity);
            //_context.Entry(entity).State = EntityState.Detached;
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
