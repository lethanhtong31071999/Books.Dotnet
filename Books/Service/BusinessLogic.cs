using Books.BusinessLogic;
using Books.BusinessLogic.IBusinessLogic;
using Books.BusinessLogic.IService;
using Books.Data;
using Books.DataAcess.Repository;
using Books.Service.IService;

namespace Books.Service
{
    public class BusinessLogic : IBusinessLogic
    {
        private readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStripeService _stripeService;
        public IProductService ProductService { get;}
        public ICompanyService CompanyService { get;}
        public IUserService UserService { get;}
        public IShoppingCartService ShoppingCartService { get;} 
        public IOrderManagementService OrderManagementService { get;}
        public BusinessLogic(IUnitOfWork unit, IWebHostEnvironment webHostEnv, IStripeService stripeService)
        {
            _unit = unit;
            _webHostEnvironment = webHostEnv;
            _stripeService = stripeService;

            ProductService = new ProductService(_unit, _webHostEnvironment);
            CompanyService = new CompanyService(_unit);
            UserService = new UserService(_unit);
            ShoppingCartService = new ShoppingCartService(_unit, _stripeService);
            OrderManagementService = new OrderManagementService(_unit, _stripeService);
        }
    }
}
