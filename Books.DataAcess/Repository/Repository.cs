using Books.Data;
using Books.DataAcess.Repository.IRepository;
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

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, bool isTrack = true, string includedProps = null)
        {
            IQueryable<T> query = _dbSet;
            query = isTrack ? query.Where(filter) : query.Where(filter).AsNoTracking();
            // Rule: "property1,property2"
            if(includedProps != null)
            {
                var props = includedProps.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                props.ForEach(x =>
                {
                    query = query.Include(x);
                });
            }
            return query.FirstOrDefault<T>();
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
