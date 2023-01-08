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
        public ShoppingCartService()
        {
        }

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
                if(applicationUser.CompanyId != null && applicationUser.CompanyId > 0)
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                    orderHeader.OrderStatus = SD.StatusApproved;
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

                // For Company user
                if(applicationUser.CompanyId != null && applicationUser.CompanyId > 0)
                {
                    obj.OrderHeader.Id = orderHeader.Id;
                    return null;
                }

                // Stripe for all type of customer except for Company
                var lineItems = new List<SessionLineItemOptions>();
                foreach (var item in shoppingCarts)
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
                var domain = "https://localhost:44330";
                var options = new SessionCreateOptions
                {
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = @$"{domain}/Customer/Cart/OrderConfirmation?id={orderHeader.Id}",
                    CancelUrl = @$"{domain}/customer/Cart/Index?orderId={orderHeader.Id}",
                };
                var service = new SessionService();
                var session = service.Create(options);

                orderHeader.SessionId = session.Id;
                _unit.OrderHeaderRepo.Update(orderHeader);
                _unit.Save();
                return session;
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
