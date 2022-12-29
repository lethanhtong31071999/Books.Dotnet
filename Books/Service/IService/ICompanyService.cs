using Books.BusinessLogic.IService;
using Books.Model;

namespace Books.Service.IService
{
    public interface ICompanyService
    {
        public Pagination<Company> HandleGetAllCompaniesWithPagination(Pagination<Company> pagination);
        public Company HandleUpsertGetMethod(int? id);
        public bool HandleUpsertPostMethod(Company obj);
        public bool HandleDeleteCompany(int id);
        public void CreateDataBasedOn1Data();
        public void DeleteData();
    }
}
