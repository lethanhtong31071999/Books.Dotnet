using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public class CategoryRepo : Repository<Category>, ICategoryRepo
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Category entity)
        {
            //_db.Categories.Update(entity);
            _db.Set<Category>().Update(entity);
        }
    }
}
