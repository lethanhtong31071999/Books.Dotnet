using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Models;
using Microsoft.EntityFrameworkCore;
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
        public void Update(Category obj)
        {
            var objFromDba = base.GetFirstOrDefault(x => x.Id == obj.Id, isTrack: false);
            if(objFromDba != null)
            {
                objFromDba.Name = obj.Name;
                objFromDba.DisplayOrder = obj.DisplayOrder;
                _db.Set<Category>().Update(objFromDba);
            }
        }
    }
}
