using Books.Data;
using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _db.Categories.ToList();
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Category was deleted successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //Update
        public IActionResult Update(int? id)
        {
            if(id == null || id == 0) return NotFound();
            var model = _db.Categories.AsNoTracking().FirstOrDefault<Category>(obj => obj.Id == id);
            if(model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category c)
        {
            if (c == null) return View(c);
            _db.Categories.Update(c);
            _db.SaveChanges();
            TempData["success"] = "Category was updated successfully!";
            return RedirectToAction("Index", "Category");
        }

        // Delete
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            var obj = _db.Categories.FirstOrDefault<Category>(c => c.Id == id);
            if (obj != null) return View(obj);
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id, bool none = true)
        {
            if (id == 0 || id == null) return NotFound();
            var obj = _db.Categories.FirstOrDefault<Category>(c => c.Id == id);
            if (obj != null)
            {
                _db.Categories.Remove(obj);
                _db.SaveChanges();
                TempData["success"] = "Category was deleted successfully!";
                return RedirectToAction("Index", "Category");
            }
            return NotFound();
        }
    }
}
