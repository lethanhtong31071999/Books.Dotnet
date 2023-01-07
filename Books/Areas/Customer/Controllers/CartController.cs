using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Utility;
using Stripe.Checkout;
using System.Security.Claims;

namespace Books.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IBusinessLogic _businessLogic;
        private ShoppingCartVM _shoppingCartVM;
        public CartController(IUnitOfWork unit, IBusinessLogic businessLogic)
        {
            _unit = unit;
            _businessLogic = businessLogic;
            Stripe.StripeConfiguration.ApiKey
                = "sk_test_51MM8LjFpSwCTDFNnB2stop6Qtxws02R3C8LOYIRm5Z66ejekrkEDcauTP0jkdxOugRxnUxGvmvTMA0IKaNmeNgl000jzQ4uK4N";
        }
        public IActionResult Index(int? orderId)
        {
            // Remove order from Dba when customer cancel payment
            if(orderId != null)
            {
                var orderHeaderFromDba = _unit.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == orderId);
                var orderDetailsFromDba = _unit.OrderDetailRepo.GetAllWithCondition(x => x.OrderHeaderId == orderId).AsEnumerable<OrderDetail>();
                if (orderHeaderFromDba != null)
                {
                    _unit.OrderHeaderRepo.Remove(orderHeaderFromDba);
                    _unit.OrderDetailRepo.RemoveRange(orderDetailsFromDba);
                    _unit.Save();
                }
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            _shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = _unit.ShoppingCartRepo
                .GetAllWithCondition(x => x.UserId == claims.Value, includedProps: "Product")
                .AsEnumerable<ShoppingCart>(),
                OrderHeader = new OrderHeader(),
            };
            foreach (var cart in _shoppingCartVM.ShoppingCarts)
            {
                cart.FinalPrice = GetPriceBasedOnQuantity(cart.Count, cart.Product);
                _shoppingCartVM.OrderHeader.OrderTotal += cart.FinalPrice * cart.Count;
                _shoppingCartVM.TotalItems += cart.Count;
            }

            return View(_shoppingCartVM);
        }

        public IActionResult Plus(int? cartId)
        {
            if (cartId != null)
            {
                var objFromDba = _unit.ShoppingCartRepo.GetFirstOrDefault(x => x.Id == cartId);
                if (objFromDba != null)
                {
                    _unit.ShoppingCartRepo.IncrementCount(objFromDba, 1);
                    _unit.Save();
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("Index");
        }

        public IActionResult Minus(int? cartId)
        {
            if (cartId != null)
            {
                var objFromDba = _unit.ShoppingCartRepo.GetFirstOrDefault(x => x.Id == cartId);
                if (objFromDba != null)
                {
                    _unit.ShoppingCartRepo.DecrementCount(objFromDba, 1);
                    _unit.Save();
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? cartId)
        {
            var objFromDba = _unit.ShoppingCartRepo.GetFirstOrDefault(x => x.Id == cartId);
            if (objFromDba != null)
            {
                _unit.ShoppingCartRepo.Remove(objFromDba);
                _unit.Save();
                TempData["success"] = $"Removed successfully";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var applicationUser = _unit.UserRepo.GetFirstOrDefault(x => x.Id == claims.Value);
            _shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = _unit.ShoppingCartRepo
                    .GetAllWithCondition(x => x.UserId == applicationUser.Id, includedProps: "Product")
                    .AsEnumerable<ShoppingCart>(),
                OrderHeader = new OrderHeader()
                {
                    Name = applicationUser.Name,
                    PhoneNumber = applicationUser.PhoneNumber,
                    StreetAddress = applicationUser.StreetAddress,
                    City = applicationUser.City,
                    State = applicationUser.State,
                    PostalCode = applicationUser.PostalCode,
                },
            };
            foreach (var cart in _shoppingCartVM.ShoppingCarts)
            {
                cart.FinalPrice = GetPriceBasedOnQuantity(cart.Count, cart.Product);
                _shoppingCartVM.TotalItems += cart.Count;
                _shoppingCartVM.OrderHeader.OrderTotal += cart.FinalPrice * cart.Count;
            }
            return View(_shoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM obj, bool isTrashparam = true)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var applicationUser = _unit.UserRepo.GetFirstOrDefault(x => x.Id == claims.Value);
            if (applicationUser != null)
            {
                var session = _businessLogic.ShoppingCartService.HandleAddSummary(obj, applicationUser);
                if (session == null) return RedirectToAction("OrderConfirmation", "Cart", new { id = obj.OrderHeader.Id });
                Response.Headers.Add("Location", session.Url);
            }
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unit.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == id, includedProps: "User");
            if (orderHeader != null && orderHeader.User != null)
            {
                // For all customers except for company
                if (orderHeader.User.CompanyId == null || orderHeader.User.CompanyId == 0)
                {
                    var service = new SessionService();
                    var session = service.Get(orderHeader.SessionId);

                    // check the stripe status
                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        _unit.OrderHeaderRepo.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                        _unit.Save();
                    }
                }

                // Remove Shopping Cart List from Dba
                var ShoppingCarts = _unit.ShoppingCartRepo
                    .GetAllWithCondition(x => x.UserId == orderHeader.UserId)
                    .AsEnumerable<ShoppingCart>();
                _unit.ShoppingCartRepo.RemoveRange(ShoppingCarts);
                _unit.Save();
            }
            return View(id);
        }



        private double GetPriceBasedOnQuantity(int count, Product p)
        {
            if (count <= 50) return p.Price;
            else if (count <= 100) return p.Price50;
            else return p.Price100;
        }
    }
}
