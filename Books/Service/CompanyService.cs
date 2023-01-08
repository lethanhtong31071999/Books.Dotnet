using Books.DataAcess.Repository;
using Books.Model;
using Books.Service.IService;

namespace Books.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unit;
        public CompanyService(IUnitOfWork unit)
        {
            _unit = unit;
        }


        public Pagination<Company>  HandleGetAllCompaniesWithPagination(Pagination<Company> paginationModel)
        {
            return _unit.CompanyRepo
                    .GetAllWithPagination(paginationModel);
        }

        public Company HandleUpsertGetMethod(int? id)
        {
            if (id == null || id == 0)
                // Create Function               
                return new Company();
            else
            {
                // Update Function
                var obj = _unit.CompanyRepo.GetFirstOrDefault(x => x.Id == id, isTracked: false);
                return obj != null ? obj : new Company();
            }
        }

        public bool HandleUpsertPostMethod(Company obj)
        {
            try
            {
                if (obj.Id == 0)
                {
                    // Create
                    _unit.CompanyRepo.Add(obj);
                    _unit.Save();
                }
                else
                {
                    // Update
                    _unit.CompanyRepo.Update(obj);
                    _unit.Save();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HandleDeleteCompany(int id)
        {
            try
            {
                var objFromDba = _unit.CompanyRepo.GetFirstOrDefault(x => x.Id == id, isTracked: true);
                if (objFromDba != null)
                {
                    _unit.CompanyRepo.Remove(objFromDba);
                    _unit.Save();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public void CreateDataBasedOn1Data()
        {
            var obj = _unit.CompanyRepo.GetFirstOrDefault(x => true);
            var companies = new List<Company>();
            for (int i = 0; i < 50; i++)
            {
                companies.Add(new Company()
                {
                    Name = obj.Name + i,
                    City = obj.City,
                    StreetAddress = obj.StreetAddress,
                    PostalCode = obj.PostalCode,
                    State = obj.State,
                    PhoneNumber = obj.PhoneNumber,
                });
            }
            _unit.CompanyRepo.AddRange(companies.AsEnumerable<Company>());
            _unit.Save();
        }
        public void DeleteData()
        {
            _unit.CompanyRepo.RemoveRange(_unit.CompanyRepo.GetAll());
            _unit.Save();
        }
    }
}
