using Books.BusinessLogic.IBusinessLogic;
using Books.BusinessLogic.IService;
using Books.Data;
using Books.DataAcess.Repository;

namespace Books.Service.IService
{
    public class CompanyService : BusinessLogic, ICompanyService
    {
        private readonly IUnitOfWork _unit;
        public CompanyService(IUnitOfWork unit, IWebHostEnvironment webHostenv) : base(unit, webHostenv)
        {
            _unit = unit;
        }

        #region API endpoint
        public bool HandleDelete(int id)
        {
            try
            {
                var objFromDba = _unit.CompanyRepo.GetFirstOrDefault(x => x.Id == id);
                if (objFromDba == null) return false;
                _unit.CompanyRepo.Remove(objFromDba);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
