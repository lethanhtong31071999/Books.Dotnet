using Books.BusinessLogic.IService;
using Microsoft.AspNetCore.Mvc;

namespace Books.Service.IService
{
    public interface ICompanyService : IBusinessLogic
    {
        public bool HandleDelete(int id);
    }
}
