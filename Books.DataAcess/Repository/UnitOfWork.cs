using Books.Data;
using Books.DataAcess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepo CategoryRepo { get; private set; }
        public ICoverTypeRepo CoverTypeRepo { get; private set; }
        public IProductRepo ProductRepo { get; private set; }   
        public ICompanyRepo CompanyRepo { get; private set; }
        public IUserRepo UserRepo { get; private set; }
        public IShoppingCartRepo ShoppingCartRepo { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepo = new CategoryRepo(_db);
            CoverTypeRepo = new CoverTypeRepo(_db);
            ProductRepo = new ProductRepo(_db);
            CompanyRepo = new CompanyRepo(_db);
            ShoppingCartRepo = new ShoppingCartRepo(_db);
            UserRepo = new UserRepo(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
