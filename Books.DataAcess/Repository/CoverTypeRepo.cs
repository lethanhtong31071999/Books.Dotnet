using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public class CoverTypeRepo : Repository<CoverType>, ICoverTypeRepo
    {
        private readonly ApplicationDbContext _db;
        public CoverTypeRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(CoverType entity)
        {
            _db.Update(entity);
        }
    }
}
