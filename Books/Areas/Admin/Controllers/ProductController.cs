using Books.BusinessLogic;
using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.PaginationModel;
using Books.Model.ViewModel;
using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IBusinessLogic _businessLogic;
        public ProductController(IBusinessLogic businessLogic)
        {
            _businessLogic = businessLogic;
        }

        public IActionResult Index()
        {
            //_businessLogic.ProductService.CreateDataBasedOn1Data();
            //_businessLogic.ProductService.DeleteData();
            return View();
        }

        // Upsert
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            return View(_businessLogic.ProductService.HandleUpsertGetMethod(id));
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (!_businessLogic.ProductService.HandleUpsertPostMethod(obj, file))
                {
                    TempData["error"] = "The values in fields are wrong!";
                    return View(obj);
                }
                if (obj.Product.Id == 0)
                {
                    // Create
                    TempData["success"] = "Product was created successfully!";
                }
                else
                {
                    // Update
                    TempData["success"] = "Product was updated successfully!";
                }

                return RedirectToAction("Index", "Product");
            }
            else
            {
                TempData["error"] = "The values in fields are wrong!";
                return View(_businessLogic.ProductService.HandleUpsertGetMethod(obj.Product.Id));
            }
        }

        // Delete
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id != null && id != 0)
            {
                var isSuccess = _businessLogic.ProductService.HandleDeleteProduct(id.Value);
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

        #region API endpoint
        [HttpPost]
        public IActionResult GetAllProducts()
        {
            var paginationModel = new Pagination<Product>()
            {
                Filter = new Filter()
                {
                    Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault()),
                    Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault()),
                    // check text search null/""
                    TextSearch = Request.Form["search[value]"].FirstOrDefault()
                },
                Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault()),
                RecordsFiltered = 0,
                RecordsTotal = 0,
                Data = null,
            };
            paginationModel = _businessLogic.ProductService.HandleGetAllProductsWithPagination(paginationModel);
            return Json(new
            {
                data = paginationModel.Data,
                recordsFiltered = paginationModel.RecordsTotal,
                recordsTotal = paginationModel.RecordsTotal,
                draw = paginationModel.Draw,
            });
        }
        #endregion
    }
}