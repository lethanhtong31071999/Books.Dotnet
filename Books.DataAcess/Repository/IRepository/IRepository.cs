using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // Get
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, bool isTrack = true);
        IEnumerable<T> GetAll();

        // Add
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);

        // Remove
        void Remove(T entity);
        void RemoveRange (IEnumerable<T> entities);
    }
}
