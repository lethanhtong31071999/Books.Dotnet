using Books.Model;
using Books.Model.ViewModel;
using Stripe.Checkout;

namespace Books.Service.IService
{
    public interface IShoppingCartService
    {
        public bool UpsertShoppingCart(ShoppingCart obj);
        public Session HandleAddSummary(ShoppingCartVM obj, User applicationUser);
    }
}
