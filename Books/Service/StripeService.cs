using Books.BusinessLogic.IBusinessLogic;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Service.IService;
using Stripe;
using Stripe.Checkout;

namespace Books.Service
{
    public class StripeService : IStripeService
    {
        private string domain = "https://localhost:44330";
        public StripeService()
        {
            Stripe.StripeConfiguration.ApiKey
                = "sk_test_51MM8LjFpSwCTDFNnB2stop6Qtxws02R3C8LOYIRm5Z66ejekrkEDcauTP0jkdxOugRxnUxGvmvTMA0IKaNmeNgl000jzQ4uK4N";
        }

        public Session Payment(IEnumerable<PaymentStripe> items, int orderHeaderId)
        {
            var lineItems = new List<SessionLineItemOptions>();
            foreach (var item in items)
            {
                lineItems.Add(new SessionLineItemOptions()
                {
                    Quantity = item.Count,
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = "usd",
                        UnitAmount = (long)item.FinalPrice * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Product.Title,
                            Description = item.Product.Description,
                        }
                    }
                });
            }
            var options = new SessionCreateOptions
            {
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = @$"{domain}/Customer/Cart/OrderConfirmation?id={orderHeaderId}",
                CancelUrl = @$"{domain}/customer/Cart/Index?orderId={orderHeaderId}",
            };
            var service = new SessionService();
            var session = service.Create(options);
            return session;
        }

        public void Refund(double amount, string paymentIntentId)
        {
            var options = new RefundCreateOptions()
            {
                Amount = (long) amount * 100,
                PaymentIntent = paymentIntentId,
                Reason = "requested_by_customer",
            };
            var service = new RefundService();
            service.Create(options);
        }
    }
}
