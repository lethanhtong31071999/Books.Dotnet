using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Microsoft.AspNetCore.Mvc;
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
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(searchTxt))
            {
                products = products.Where(x => x.Title.ToLower().Contains(searchTxt.ToLower()));
                page = 1;
                ViewBag.CurrentSearch = searchTxt;
            }

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Details(int? id)
            {
            if (id != null || id != 0)
            {
                var product = _unit.ProductRepo
                    .GetFirstOrDefault(x => x.Id == id, isTrack: false, includedProps: "Category,CoverType");
                if (product != null)
                {
                    return View(new ShoppingCart ()
                    {
                        Count = 1,
                        Product = product,
                    });
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("Index", "Home");
        }
    }
}