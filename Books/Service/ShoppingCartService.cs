using Books.DataAcess.Repository;
using Books.Service.IService;

namespace Books.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unit;
        public ShoppingCartService(IUnitOfWork unit)
        {
            _unit = unit;
        }
    }
}
