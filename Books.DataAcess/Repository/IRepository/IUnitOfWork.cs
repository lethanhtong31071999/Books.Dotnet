using Books.DataAcess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public interface IUnitOfWork
    {
        public ICategoryRepo CategoryRepo { get; }
        public ICoverTypeRepo CoverTypeRepo { get; }
        public void Save();
    }
}
