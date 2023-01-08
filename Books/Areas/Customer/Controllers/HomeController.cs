using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Utility;
using System.Security.Claims;
using X.PagedList;

namespace Books.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IBusinessLogic _businessLogic;
        private readonly IUnitOfWork _unit;

        public HomeController(IBusinessLogic businessLogic, IUnitOfWork unit)
        {
            _businessLogic = businessLogic;
            _unit = unit;
        }

        public IActionResult Index(int? page, string searchTxt)
        {
            var products = _unit.ProductRepo.GetAll();
            int pageSize = SD.MaximumDisplayProduct;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(searchTxt))
            {
                products = products.Where(x => x.Title.ToLower().Contains(searchTxt.ToLower()));
                page = 1;
                ViewBag.CurrentSearch = searchTxt;
            }

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult Details(int? productId)
            {
            if (productId != null || productId > 0)
            {
                var product = _unit.ProductRepo
                    .GetFirstOrDefault(x => x.Id == productId, isTracked: false, includedProps: "Category,CoverType");
                if (product != null)
                {
                    return View(new ShoppingCart ()
                    {
                        Count = 1,
                        ProductId = product.Id,
                        Product = product,
                    });
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart obj)
        {
            if(obj.Count > 0 && obj.ProductId != 0)
            {
                var clamsIdentity = (ClaimsIdentity) User.Identity;
                var claim = clamsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                obj.UserId = claim.Value;
                if(_businessLogic.ShoppingCartService.UpsertShoppingCart(obj))
                {
                    var product = _unit.ProductRepo.GetFirstOrDefault(x => x.Id == obj.ProductId);
                    TempData["success"] = $"Added {product.Title} with {obj.Count} items to cart successfully!";
                }
                else
                    TempData["error"] = "Something went wrong!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}