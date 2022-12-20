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
        public ICoverTypeRepo CoverRepo { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepo = new CategoryRepo(_db);
            CoverRepo = new CoverTypeRepo(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
