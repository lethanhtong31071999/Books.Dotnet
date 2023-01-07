
using Books.BusinessLogic.IBusinessLogic;
using Books.Service.IService;

namespace Books.BusinessLogic.IService
{
    public interface IBusinessLogic
    {
        public IProductService ProductService { get; }
        public ICompanyService CompanyService { get; } 
        public IUserService UserService { get; }
        public IShoppingCartService ShoppingCartService { get; }
        public IOrderManagementService OrderManagementService { get; }
    }
}
