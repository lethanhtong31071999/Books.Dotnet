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
        public IProductRepo ProductRepo { get; }
        public ICompanyRepo CompanyRepo { get; }
        public IUserRepo UserRepo { get; }
        public IShoppingCartRepo ShoppingCartRepo { get; }
        public IOrderDetailRepo OrderDetailRepo { get; }
        public IOrderHeaderRepo OrderHeaderRepo { get; }
        public void Save();
    }
}
