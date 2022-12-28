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

        public Pagination<Company> GetAllWithPagination(Pagination<Company> pagingModel, string includedProps = null)
        {
            IQueryable<Company> query = _db.Company;
            pagingModel.RecordsTotal = query.Count();
            var textSearch = pagingModel.Filter.TextSearch;
            if (textSearch != null && textSearch.Trim().Length > 0)
            {
                query = query
                    .Where(x => x.Name.Contains(textSearch))
                    .Skip(pagingModel.Filter.Start).Take(pagingModel.Filter.Length);
                pagingModel.Filter.Start = 0;
                pagingModel.RecordsFiltered = query.Count();
                pagingModel.RecordsTotal = query.Count();
            }
            else
            {
                query = query.Skip(pagingModel.Filter.Start).Take(pagingModel.Filter.Length);
            }
            query = base.IncludeProperty(query, includedProps);
            pagingModel.Data = query;
            return pagingModel;
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
