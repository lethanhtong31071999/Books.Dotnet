using Books.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Admin.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                if (_companyService.HandleDelete(id.Value))
                    TempData["success"] = "The company was deleted Successfully";
                else
                    TempData["error"] = "Something went wrong!";
            }
            return RedirectToAction("Index", "Company");
        }
        #endregion
    }
}
