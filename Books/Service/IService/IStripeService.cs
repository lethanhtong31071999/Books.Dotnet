using Books.BusinessLogic.IService;
using Books.Model;
using Stripe.Checkout;

namespace Books.Service.IService
{
    public interface IStripeService
    {
        public Session Payment(IEnumerable<PaymentStripe> shoppingCarts, int orderHeaderId);
        public void Refund(double amount, string paymentIntentId);
    }
}
