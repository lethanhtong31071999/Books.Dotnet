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
    public class CompanyRepo : Repository<Company>, ICompanyRepo
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        
        public void Update(Company obj)
        {
            if (obj == null) return;
            var objFromDba = base.GetFirstOrDefault(x => x.Id == obj.Id, isTrack: false);
            if(objFromDba != null)
            {
                objFromDba.Name = obj.Name;
                objFromDba.StreetAddress = obj.StreetAddress;
                objFromDba.City = obj.City;
                objFromDba.State = obj.State;
                objFromDba.PhoneNumber = obj.PhoneNumber;
                objFromDba.PostalCode = obj.PostalCode;
                _db.Company.Update(objFromDba);
            }
        }
    }
}
