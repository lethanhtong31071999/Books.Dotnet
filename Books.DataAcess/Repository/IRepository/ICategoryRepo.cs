using Books.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository.IRepository
{
    public interface ICategoryRepo : IRepository<Category>
    {
        public void Update(Category entity);
    }
}
