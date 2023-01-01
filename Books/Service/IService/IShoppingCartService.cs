using Books.Model;

namespace Books.Service.IService
{
    public interface IShoppingCartService
    {
        public bool UpsertShoppingCart(ShoppingCart obj);
    }
}
