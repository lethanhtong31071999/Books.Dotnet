using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.PaginationModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Utility;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IBusinessLogic _businessLogic;
        public CompanyController(IBusinessLogic businessLogic)
        {
            _businessLogic = businessLogic;
        }

        public IActionResult Index()
        {
            //_businessLogic.CompanyService.CreateDataBasedOn1Data();
            return View();
        }
        
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            return View(_businessLogic.CompanyService.HandleUpsertGetMethod(id));

        }
        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                // Error
                if (!_businessLogic.CompanyService.HandleUpsertPostMethod(obj))
                {
                    TempData["error"] = "The values in fields are wrong!";
                    return View(obj);
                }
                // Success
                if (obj.Id == 0)
                {
                    // Create
                    TempData["success"] = "Product was created successfully!";
                }
                else
                {
                    // Update
                    TempData["success"] = "Product was updated successfully!";
                }

                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "The values in fields are wrong!";
                return View(obj);
            }
        }

        #region API endpoint
        [HttpPost]
        public IActionResult GetAllCompanies()
        {
            var paginationModel = new Pagination<Company>()
            {
                Filter = new Filter()
                {
                    Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault()),
                    Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault()),
                    TextSearch = Request.Form["search[value]"].FirstOrDefault()
                },
                Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault()),
                RecordsFiltered = 0,
                RecordsTotal = 0,
                Data = null,
            };
            paginationModel = _businessLogic.CompanyService.HandleGetAllCompaniesWithPagination(paginationModel);
            return Json(new
            {
                data = paginationModel.Data,
                recordsFiltered = paginationModel.RecordsTotal,
                recordsTotal = paginationModel.RecordsTotal,
                draw = paginationModel.Draw,
            });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id != null && id != 0)
            {
                var isSuccess = _businessLogic.CompanyService.HandleDeleteCompany(id.Value);
                if (isSuccess)
                    return Json(new
                    {
                        success = true,
                        message = "Deleted successfully!"
                    });
            }
            return Json(new
            {
                success = false,
                message = "Something went wrong!"
            });
        }
        #endregion
    }
}
