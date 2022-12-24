using Books.BusinessLogic;
using Books.BusinessLogic.IBusinessLogic;
using Books.BusinessLogic.IService;
using Books.Data;
using Books.DataAcess.Repository;

namespace Books.Service
{
    public class BusinessLogic : IBusinessLogic
    {
        private readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IProductService ProductService { get;}

        public BusinessLogic(IUnitOfWork unit, IWebHostEnvironment webHostEnv)
        {
            _unit = unit;
            _webHostEnvironment = webHostEnv;
            ProductService = new ProductService(_unit, _webHostEnvironment);
        }
    }
}
