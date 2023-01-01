using Books.DataAcess.Repository;
using Books.Model;
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

        public bool UpsertShoppingCart(ShoppingCart obj)
        {
            try
            {
                var objFromDba = _unit.ShoppingCartRepo
                    .GetFirstOrDefault(x => x.ProductId == obj.ProductId && x.UserId == obj.UserId);
                if (objFromDba == null)
                {
                    // Add new
                    _unit.ShoppingCartRepo.Add(obj);
                }
                else
                {
                    // Incresement count
                    _unit.ShoppingCartRepo.IncrementCount(objFromDba, obj.Count);
                }
                _unit.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
