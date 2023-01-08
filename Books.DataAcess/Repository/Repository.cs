using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private DbSet<T> _dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public IQueryable<T> IncludeProperty(IQueryable<T> query, string? includedProps)
        {
            // Rule: "property1,property2"
            if (includedProps != null)
            {
                var props = includedProps.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                props.ForEach(x =>
                {
                    query = query.Include(x);
                });
            }
            return query;
        }

        // Add
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        // Get
        public IEnumerable<T> GetAll(string includedProps = null)
        {
            IQueryable<T> query = _dbSet;
            return IncludeProperty(query, includedProps);
        }

        public IEnumerable<T> GetAllWithCondition(Expression<Func<T, bool>> filter, string includedProps = null)
        {
            IQueryable<T> query = _dbSet.Where<T>(filter);
            return IncludeProperty(query, includedProps);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, bool isTracked = true, string includedProps = null)
        {
            IQueryable<T> query = _dbSet;
            if(!isTracked)
            {
                query = query.AsNoTracking();
            }
            query = query.Where(filter);
            return IncludeProperty(query, includedProps).FirstOrDefault<T>();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
