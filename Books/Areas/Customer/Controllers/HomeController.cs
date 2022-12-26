using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}