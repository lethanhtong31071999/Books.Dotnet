using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Model;
using Microsoft.EntityFrameworkCore;
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
        public void Update(CoverType obj)
        {
            var objFromDba = base.GetFirstOrDefault(x => x.Id == obj.Id, isTracked:false);
            if(objFromDba != null)
            {
                objFromDba.Name = obj.Name;
                _db.Update(objFromDba);
            }
        }
    }
}
