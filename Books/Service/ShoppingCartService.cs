using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.ViewModel;
using Books.Service.IService;
using Model.Utility;
using Stripe.Checkout;

namespace Books.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IStripeService _stripeService;
        private readonly IUnitOfWork _unit;
        public ShoppingCartService(IUnitOfWork unit, IStripeService stripeService)
        {
            _unit = unit;
            _stripeService = stripeService;
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

        public Session HandleAddSummary(ShoppingCartVM obj, User applicationUser)
        {
            try
            {
                // Set up list item
                var shoppingCarts = _unit.ShoppingCartRepo
                        .GetAllWithCondition(x => x.UserId == applicationUser.Id, includedProps: "Product");

                // Set up order header
                var orderHeader = new OrderHeader()
                {
                    PaymentStatus = SD.PaymentStatusPending,
                    OrderStatus = SD.StatusPending,
                    OrderDate = DateTime.Now,
                    UserId = applicationUser.Id,
                    Name = applicationUser.Name,
                    PhoneNumber = applicationUser.PhoneNumber,
                    StreetAddress = applicationUser.StreetAddress,
                    City = applicationUser.City,
                    State = applicationUser.State,
                    PostalCode = applicationUser.PostalCode,
                    SessionId = "",
                    PaymentIntentId = "",
                };

                // Company dont need to pay for orders in advance
                if (applicationUser.CompanyId != null && applicationUser.CompanyId > 0)
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                    orderHeader.OrderStatus = SD.StatusPending;
                }

                // Add OrderHeader
                foreach (var cart in shoppingCarts)
                {
                    cart.FinalPrice = GetPriceBasedOnQuantity(cart.Count, cart.Product);
                    orderHeader.OrderTotal += cart.FinalPrice * cart.Count;
                }
                _unit.OrderHeaderRepo.Add(orderHeader);
                _unit.Save();

                // Add OrderDetails
                var orderDetails = new List<OrderDetail>();
                foreach (var item in shoppingCarts)
                {
                    orderDetails.Add(new OrderDetail()
                    {
                        ProductId = item.ProductId,
                        OrderHeaderId = orderHeader.Id,
                        Count = item.Count,
                        FinalPrice = item.FinalPrice,
                    });
                }
                _unit.OrderDetailRepo.AddRange(orderDetails.AsEnumerable<OrderDetail>());
                _unit.Save();

                // Stripe
                if (applicationUser.CompanyId != null && applicationUser.CompanyId > 0)
                {
                    // For Company user
                    obj.OrderHeader.Id = orderHeader.Id;
                    var ShoppingCarts = _unit.ShoppingCartRepo
                        .GetAllWithCondition(x => x.UserId == orderHeader.UserId)
                        .AsEnumerable<ShoppingCart>();
                    _unit.ShoppingCartRepo.RemoveRange(ShoppingCarts);
                    _unit.Save();
                    return null;
                }
                else
                {
                    // For all type of customer except for Company
                    var paymentStripe = shoppingCarts.Select(x => new PaymentStripe()
                    {
                        Product = x.Product,
                        Count = x.Count,
                        FinalPrice= x.FinalPrice,
                    }).AsEnumerable();
                    var session = _stripeService.Payment(paymentStripe, orderHeader.Id);
                    orderHeader.SessionId = session.Id;
                    _unit.OrderHeaderRepo.Update(orderHeader);
                    _unit.Save();
                    return session;
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        private double GetPriceBasedOnQuantity(int count, Product p)
        {
            if (count <= 50) return p.Price;
            else if (count <= 100) return p.Price50;
            else return p.Price100;
        }
    }
}
