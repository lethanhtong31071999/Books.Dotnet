using Books.Data;
using Books.DataAcess.Repository;
using Books.DataAcess.Repository.IRepository;
using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unit;
        public CategoryController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public IActionResult Index()
        {
            var categories = _unit.CategoryRepo.GetAll().ToList();
            return View(categories);
        }

        //Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name cannot exactly match with Display Order");
            }
            if(ModelState.IsValid)
            {
                _unit.CategoryRepo.Add(obj);
                _unit.Save();
                TempData["success"] = "Category was deleted successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //Update
        public IActionResult Update(int? id)
        {
            if(id == null || id == 0) return NotFound();
            var model = _unit.CategoryRepo.GetFirstOrDefault(obj => obj.Id == id);
            if(model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category obj)
        {
            if (obj == null) return View(obj);
            _unit.CategoryRepo.Update(obj);
            _unit.Save();
            TempData["success"] = "Category was updated successfully!";
            return RedirectToAction("Index", "Category");
        }

        // Delete
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            var obj = _unit.CategoryRepo.GetFirstOrDefault(obj => obj.Id == id);
            if (obj != null) return View(obj);
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id, bool none = true)
        {
            if (id == 0 || id == null) return NotFound();
            var obj = _unit.CategoryRepo.GetFirstOrDefault(c => c.Id == id);
            if (obj != null)
            {
                _unit.CategoryRepo.Remove(obj);
                _unit.Save();
                TempData["success"] = "Category was deleted successfully!";
                return RedirectToAction("Index", "Category");
            }
            return NotFound();
        }
    }
}
