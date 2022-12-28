using Books.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository.IRepository
{
    public interface ICompanyRepo : IRepository<Company>
    {
        public void Update(Company obj);
        public Pagination<Company> GetAllWithPagination(Pagination<Company> pagingModel, string includedProps = null);
    }
}
