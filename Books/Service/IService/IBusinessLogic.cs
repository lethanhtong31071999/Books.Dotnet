using Books.BusinessLogic.IBusinessLogic;

namespace Books.BusinessLogic.IService
{
    public interface IBusinessLogic
    {
        public IProductService ProductService { get; }
    }
}
