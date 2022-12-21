using Books.DataAcess.Repository;
using Books.Model;
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
            if (id == null || id == 0)
            {
                // Create Function
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
                ViewBag.Category = categories;           
                ViewBag.CoverType = coverTypes;

                return View(new Product());
            }
            else
            {
                // Update Function
                var obj = _unit.ProductRepo.GetFirstOrDefault(x => x.Id == id, isTrack: false);
                return View(obj);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Product obj)
        {
            if (obj.Id == 0)
            {
                // Create Function
                if (ModelState.IsValid)
                {
                    _unit.ProductRepo.Add(obj);
                    _unit.Save();
                    TempData["success"] = "Product was created successfully!";
                }
                else
                    TempData["error"] = "It failed to create the product!";
            }
            else
            {
                // Update Function
                if (!ModelState.IsValid)
                {
                    _unit.ProductRepo.Update(obj);
                    _unit.Save();
                    TempData["success"] = "Product was updated successfully!";
                }
                else
                {
                    TempData["error"] = "Something went wrong!";
                    return View(obj);
                }
            }
            return RedirectToAction("Index");
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
