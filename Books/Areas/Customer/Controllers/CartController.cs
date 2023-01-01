using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            _shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = _unit.ShoppingCartRepo
                .GetAllWithCondition(x => x.UserId == claims.Value, includedProps: "Product")
                .AsEnumerable<ShoppingCart>(),
            };
            foreach (var cart in _shoppingCartVM.ShoppingCarts)
            {
                cart.FinalPrice = GetPriceBasedOnQuantity(cart.Count, cart.Product);
                _shoppingCartVM.TotalPrice += cart.FinalPrice * cart.Count;
                _shoppingCartVM.TotalItems += cart.Count;
            }

            return View(_shoppingCartVM);
        }

        public IActionResult Plus(int? cartId)
        {
            if(cartId != null)
            {
                var objFromDba = _unit.ShoppingCartRepo.GetFirstOrDefault(x => x.Id == cartId);
                if(objFromDba != null)
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

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            _shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = _unit.ShoppingCartRepo
                    .GetAllWithCondition(x => x.UserId == claims.Value, includedProps: "Product")
                    .AsEnumerable<ShoppingCart>(),
            };
            foreach (var cart in _shoppingCartVM.ShoppingCarts)
            {
                cart.FinalPrice = GetPriceBasedOnQuantity(cart.Count, cart.Product);
                _shoppingCartVM.TotalPrice += cart.FinalPrice * cart.Count;
                _shoppingCartVM.TotalItems += cart.Count;
            }
            return View(_shoppingCartVM);
        }

        private double GetPriceBasedOnQuantity(int count, Product p)
        {
            if (count <= 50) return p.Price;
            else if (count <= 100) return p.Price50;
            else return p.Price100;
        }
    }
}
