using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.ViewModel;
using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unit;
        public ProductController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Upsert
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            var viewModel = new ProductVM();
            IEnumerable<SelectListItem> categories = _unit.CategoryRepo
                    .GetAll()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });
            IEnumerable<SelectListItem> coverTypes = _unit.CoverTypeRepo
                .GetAll()
                .Select(x =>

                     new SelectListItem()
                     {
                         Text = x.Name,
                         Value = x.Id.ToString(),
                     }
                );

            if (id == null || id == 0)
            {
                // Create Function               
                viewModel.Product = new Product();
                viewModel.CoverTypeSelectList = coverTypes;
                viewModel.CategorySelectList = categories;
            }
            else
            {
                // Update Function
                var obj = _unit.ProductRepo.GetFirstOrDefault(x => x.Id == id, isTrack: false);
                if (obj != null)
                {
                    viewModel.Product = obj;
                    viewModel.CoverTypeSelectList = coverTypes;
                    viewModel.CategorySelectList = categories;
                }
                else
                    TempData["error"] = "Something went wrong!";
            }

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (obj.Product.Id == 0)
                {
                    // Create
                    _unit.ProductRepo.Add(obj.Product);
                    _unit.Save();
                    TempData["success"] = "Product was created successfully!";
                }
                else
                {
                    // Update
                    _unit.ProductRepo.Update(obj.Product);
                    _unit.Save();
                    TempData["success"] = "Product was updated successfully!";
                }
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Something went wrong!";
                return View(obj);
            }
        }

        // Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var obj = _unit.ProductRepo.GetFirstOrDefault(x => x.Id == id);
            return obj != null ? View(obj) : NotFound();
        }
        [HttpPost]
        public IActionResult Delete(int? id, bool trashParam = true)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "It is not a exsisting product!";
                return View();
            }
            var objFromDba = _unit.ProductRepo.GetFirstOrDefault(x => x.Id == id, isTrack: true);
            if (objFromDba != null)
            {
                _unit.ProductRepo.Remove(objFromDba);
                _unit.Save();
                TempData["success"] = "Product was deleted successfully!";
                return RedirectToAction("Index");
            }
            TempData["error"] = "It failed to delete the product!";
            return View();
        }
    }
}
